using AvibaWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.CollectionViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Globalization;

namespace AvibaWeb.Controllers
{
    [Authorize]
    public class CollectionController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private const int PageSize = 10;

        public CollectionController(AppIdentityDbContext db,
            UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
        }

        public ActionResult Index()
        {
            var userId = _userManager.GetUserId(User);

            var collectionList = (from collection in _db.Collections
                    .Include(c => c.Provider).Include(c => c.DeskIssued).Include(c => c.Collector)
                join operation in _db.CollectionOperations on collection.CollectionId equals operation.CollectionId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                where collection.CollectorId == userId && operation.OperationTypeId == CollectionOperationType.COType.New
                orderby operation.OperationDateTime descending
                select new { collection, operation }).ToList();

            var model = new CollectionInfoModel
                {
                    IncomingCollections = collectionList.Count
                };

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult IncomingCollections(int? page)
        {
            var userId = _userManager.GetUserId(User);

            var collectionList = (from collection in _db.Collections
                    .Include(c => c.Provider).Include(c => c.DeskIssued).Include(c => c.Collector)
                join operation in _db.CollectionOperations on collection.CollectionId equals operation.CollectionId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                where collection.CollectorId == userId && operation.OperationTypeId == CollectionOperationType.COType.New
                orderby operation.OperationDateTime descending
                select new { collection, operation }).ToList();

            var model = (from q in collectionList
                         select new CollectionListViewModel
                {
                    CollectionId = q.collection.CollectionId,
                    CollectionOperationId = q.operation.CollectionOperationId,
                    Amount = q.collection.Amount,
                    ProviderName = q.collection.Provider != null ? q.collection.Provider.Name : "",
                    DeskId = q.collection.DeskIssued != null ? q.collection.DeskIssued.DeskId : "",
                    DeskName = q.collection.DeskIssued != null ? q.collection.DeskIssued.Description : "",
                    CollectorName = q.collection.Collector.Name,
                    IssuedDateTime = q.operation.OperationDateTime,
                    PaymentType = q.collection.PaymentType,
                    Comment = q.collection.Comment
                }).ToList();

            var pageNumber = (page ?? 1);
            return PartialView(model.ToPagedList(pageNumber, PageSize));
        }

        [HttpGet]
        public ActionResult AcceptedCollections(int? page)
        {
            var userId = _userManager.GetUserId(User);

            var collectionList = (from collection in _db.Collections
                    .Include(c => c.Provider).Include(c => c.DeskIssued).Include(c => c.Collector)
                join operation in _db.CollectionOperations on collection.CollectionId equals operation.CollectionId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                where collection.CollectorId == userId && operation.OperationTypeId == CollectionOperationType.COType.Accepted
                orderby operation.OperationDateTime descending
                select new { collection, operation }).ToList();

            var model = (from q in collectionList
                         select new CollectionListViewModel()
                {
                    CollectionId = q.collection.CollectionId,
                    Amount = q.collection.Amount,
                    ProviderName = q.collection.Provider != null ? q.collection.Provider.Name : "",
                    DeskId = q.collection.DeskIssued != null ? q.collection.DeskIssued.DeskId : "",
                    DeskName = q.collection.DeskIssued != null ? q.collection.DeskIssued.Description : "",
                    CollectorName = q.collection.Collector.Name,
                    IssuedDateTime = q.operation.OperationDateTime,
                    PaymentType = q.collection.PaymentType,
                    Comment = q.collection.Comment
                         }).ToList();

            var pageNumber = (page ?? 1);
            return PartialView(model.ToPagedList(pageNumber, PageSize));
        }

        [HttpGet]
        public ActionResult IssuedCollections(int? page, bool isAdmin)
        {
            var userId = _userManager.GetUserId(User);
            //var isUserAdmin = User.IsInRole("Administrators");

            var collectionList = (from collection in _db.Collections
                    .Include(c => c.Provider).Include(c => c.DeskIssued).Include(c => c.Collector)
                join operation in _db.CollectionOperations on collection.CollectionId equals operation.CollectionId into
                    operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                where isAdmin || collection.ProviderId == userId
                orderby operation.OperationDateTime descending
                select new {collection, operation}).ToList();

            var model = (from q in collectionList
                         select new CollectionListViewModel
                {
                    CollectionId = q.collection.CollectionId,
                    Amount = q.collection.Amount,
                    ProviderName = q.collection.Provider != null ? q.collection.Provider.Name : "",
                    DeskId = q.collection.DeskIssued != null ? q.collection.DeskIssued.DeskId : "",
                    DeskName = q.collection.DeskIssued != null ? q.collection.DeskIssued.Description : "",
                    CollectorName = q.collection.Collector.Name,
                    Status = q.operation.OperationTypeId,
                    IssuedDateTime = q.operation.OperationDateTime,
                    PaymentType = q.collection.PaymentType,
                    Comment = q.collection.Comment
                         }).ToList();

            var pageNumber = (page ?? 1);
            return PartialView(model.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        public async Task<ActionResult> AcceptCollection(int id)
        {
            var collection = await _db.Collections.Include(c => c.Provider).Include(c => c.Collector)
                .FirstOrDefaultAsync(c => c.CollectionId == id);
            if (collection == null) return PartialView("Error", new [] { "Инкассация не найдена" });

            var collectionAmount = collection.Amount;
            if (collection.Provider != null)
            {
                collection.Provider.Balance -= collectionAmount;
            }
            collection.Collector.Balance += collectionAmount;

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.Accepted
            };

            _db.CollectionOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("IncomingCollections");
        }

        [HttpPost]
        public async Task<ActionResult> RejectCollection(int id)
        {
            var collection = await _db.Collections.Include(c => c.Operations).Include(c => c.Provider)
                .Include(c => c.Collector)
                .FirstOrDefaultAsync(c => c.CollectionId == id);
            if (collection == null) return PartialView("Error", new string[] { "Инкассация не найдена" });

            var newOperation = collection.Operations
                .OrderByDescending(o => o.OperationDateTime).Take(1)
                .FirstOrDefault(o => o.OperationTypeId == CollectionOperationType.COType.New);
            if (newOperation == null) return PartialView("Error", new string[] { "Отказ невозможен" });

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.Rejected
            };

            _db.CollectionOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("IncomingCollections");
        }

        [HttpPost]
        public async Task<ActionResult> CancelCollection(int id)
        {
            var collection = await _db.Collections.Include(c => c.Operations).Include(c => c.Provider)
                .Include(c => c.Collector)
                .FirstOrDefaultAsync(c => c.CollectionId == id);
            if (collection == null) return PartialView("Error", new string[] { "Инкассация не найдена" });

            var acceptedOperation = collection.Operations
                .OrderByDescending(o => o.OperationDateTime).Take(1)
                .FirstOrDefault(o => o.OperationTypeId == CollectionOperationType.COType.Accepted);
            if (acceptedOperation == null) return PartialView("Error", new string[] { "Инкассация не принята" });

            var collectionAmount = collection.Amount;
            if (collection.Provider != null)
            {
                collection.Provider.Balance += collectionAmount;
            }
            collection.Collector.Balance -= collectionAmount;

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.New
            };

            _db.CollectionOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("AcceptedCollections");
        }

        //[ChildActionOnly]
        public async Task<ActionResult> Balance()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var model = new BalanceModel();
            if (user == null) return PartialView(model);

            model.Balance = user.Balance;
            model.IncomingNotAccepted =
                (from c in _db.Collections
                    join co in _db.CollectionOperations on c.CollectionId equals co.CollectionId into operations
                    from co in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                    where c.CollectorId == userId && co.OperationTypeId == CollectionOperationType.COType.New
                    select c.Amount).DefaultIfEmpty().Sum();
            model.IssuedNotAccepted =
                (from c in _db.Collections
                    join co in _db.CollectionOperations on c.CollectionId equals co.CollectionId into operations
                    from co in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                    where c.ProviderId == userId && co.OperationTypeId == CollectionOperationType.COType.New
                    select c.Amount).DefaultIfEmpty().Sum();
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult CreateCollection(bool isCustom = false)
        {
            var collectorsRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Инкассаторы"));

            var model = new CreateCollectionModel
            {
                Collectors = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == collectorsRole.Id)).Select(u =>
                    new SelectListItem
                    {
                        Value = u.Id,
                        Text = u.Name
                    }),
                ProviderId = isCustom ? null : _userManager.GetUserId(User),                
                RedirectAction = isCustom ? "IssuedCustomCollections" : "IssuedCollections"
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateCollection(CreateCollectionModel model)
        {
            var collection = new Collection
            {
                Amount = model.Amount,
                PaymentType = model.PaymentType,
                ProviderId = model.ProviderId,
                CollectorId = model.SelectedCollector,
                Comment = model.Comment
            };

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.New
            };

            _db.CollectionOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction(model.RedirectAction);
        }

        #region Custom Collections
        [HttpGet]
        public ActionResult IssuedCustomCollections(int? page)
        {
            var userId = _userManager.GetUserId(User);
            var isUserAdmin = User.IsInRole("Administrators");

            var collectionList = (from collection in _db.Collections
                    .Include(c => c.Provider).Include(c => c.DeskIssued).Include(c => c.Collector)
                                  join operation in _db.CollectionOperations on collection.CollectionId equals operation.CollectionId into
                                      operations
                                  from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                                  where collection.Provider == null
                                  orderby operation.OperationDateTime descending
                                  select new { collection, operation }).ToList();

            var model = (from q in collectionList
                         select new CollectionListViewModel
                         {
                             CollectionId = q.collection.CollectionId,
                             Amount = q.collection.Amount,
                             CollectorName = q.collection.Collector.Name,
                             Status = q.operation.OperationTypeId,
                             IssuedDateTime = q.operation.OperationDateTime,
                             PaymentType = q.collection.PaymentType,
                             Comment = q.collection.Comment
                         }).ToList();

            var pageNumber = (page ?? 1);
            return PartialView(model.ToPagedList(pageNumber, PageSize));
        }

        [HttpPost]
        public async Task<ActionResult> CancelCustomCollection(int id)
        {
            var collection = await _db.Collections.Include(c => c.Operations).Include(c => c.Provider)
                .Include(c => c.Collector)
                .FirstOrDefaultAsync(c => c.CollectionId == id);
            if (collection == null) return PartialView("Error", new string[] { "Инкассация не найдена" });

            var newOperation = collection.Operations
                .OrderByDescending(o => o.OperationDateTime).Take(1)
                .FirstOrDefault(o => o.OperationTypeId == CollectionOperationType.COType.New);
            if (newOperation == null) return PartialView("Error", new string[] { "Отказ невозможен" });

            var operation = new CollectionOperation
            {
                Collection = collection,
                OperationDateTime = DateTime.Now,
                OperationTypeId = CollectionOperationType.COType.Cancelled
            };

            _db.CollectionOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("IssuedCustomCollections");
        }
        #endregion

        [HttpGet]
        public async Task<ActionResult> OfficeBalance(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var model = new OfficeBalanceViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                CurrentAmount = user.Balance,
                CurrentAmountStr = user.Balance.ToString("#,0.00", nfi),
                Records = (from o in _db.OfficeBalanceHistory
                           where o.SaveDateTime.Date >= queryFromDate && o.SaveDateTime.Date <= queryToDate
                           orderby o.SaveDateTime descending
                           select new OfficeBalanceRecord
                           {
                               SaveDateTime = o.SaveDateTime.ToString("d"),
                               Balance = o.Balance.ToString("#,0.00", nfi)
                           }).ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> SaveOfficeBalance(decimal balance)
        {
            var officeBalanceRecord = new OfficeBalanceHistory
            {
                SaveDateTime = DateTime.Now,
                Balance = balance
            };

            _db.OfficeBalanceHistory.Add(officeBalanceRecord);

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }
    }
}