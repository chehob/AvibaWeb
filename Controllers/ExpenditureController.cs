using AvibaWeb.Models;
using System;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.ExpenditureViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using X.PagedList;
using System.Globalization;
using System.Collections.Generic;

namespace AvibaWeb.Controllers
{
    public class ExpenditureController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private const int PageSize = 10;

        public ExpenditureController(AppIdentityDbContext db)
        {
            _db = db;
        }

        // GET: Expenditures
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult IssuedExpenditures()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var expenditureList = (from expenditure in _db.Expenditures
                    .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                    .Where(e => e.PaymentTypeId == PaymentTypes.Cash)
                join eo in _db.ExpenditureOperations on expenditure.ExpenditureId equals eo.ExpenditureId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby operation.OperationDateTime descending
                select new { expenditure, operation }).ToList();

            var model = new ExpendituresViewModel
            {
                Items = (from e in expenditureList
                         select new ExpenditureViewItem
                         {
                             ExpenditureId = e.expenditure.ExpenditureId,
                             Amount = e.expenditure.Amount.ToString("#,0.00", nfi),
                             Name = e.expenditure.Name,
                             DeskGroup = e.expenditure.DeskGroup != null ? e.expenditure.DeskGroup.Name : "",
                             Type = e.expenditure.Type != null ? e.expenditure.Type.Description : "",
                             Object = e.expenditure.Object != null ? e.expenditure.Object.Description : "",
                             IssuedDateTime = e.operation.OperationDateTime,
                             Status = e.operation.OperationTypeId
                         }).ToList()
            };
            
            return PartialView(model);
        }

        //[HttpGet]
        //public ActionResult CashlessExpenditures(int? page)
        //{
        //    var expenditureList = (from expenditure in _db.FinancialAccountOperations
        //            //.Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
        //            where expenditure.Amount < 0
        //                           join eo in _db.ExpenditureOperations on expenditure.ExpenditureId equals eo.ExpenditureId into operations
        //                           from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
        //                           orderby operation.OperationDateTime descending
        //                           select new { expenditure, operation }).ToList();

        //    var model = (from e in expenditureList
        //                 select new ExpenditureListViewModel
        //                 {
        //                     ExpenditureId = e.expenditure.ExpenditureId,
        //                     Amount = e.expenditure.Amount,
        //                     Name = e.expenditure.Name,
        //                     DeskGroup = e.expenditure.DeskGroup != null ? e.expenditure.DeskGroup.Name : "",
        //                     Type = e.expenditure.Type != null ? e.expenditure.Type.Description : "",
        //                     Object = e.expenditure.Object != null ? e.expenditure.Object.Description : "",
        //                     IssuedDateTime = e.operation.OperationDateTime,
        //                     Status = e.operation.OperationTypeId
        //                 }).ToList();

        //    var pageNumber = (page ?? 1);
        //    return PartialView(model.ToPagedList(pageNumber, PageSize));
        //}

        [HttpGet]
        public ActionResult CreateExpenditure()
        {
            var model = new CreateExpenditureModel()
            {
                DeskGroups = from d in _db.DeskGroups.Where(g => g.IsActive).OrderBy(g => g.Name)
                    select new SelectListItem
                    {
                        Value = d.DeskGroupId.ToString(),
                        Text = d.Name
                    },
                Types = from t in _db.ExpenditureTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
                    select new SelectListItem
                    {
                        Value = t.ExpenditureTypeId.ToString(),
                        Text = t.Description
                    },
                Objects = from o in _db.ExpenditureObjects.Where(o => o.IsActive).OrderBy(o => o.Description)
                    select new SelectListItem
                    {
                        Value = o.ExpenditureObjectId.ToString(),
                        Text = o.Description
                    }
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateExpenditure(CreateExpenditureModel model)
        {
            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("IssuedExpenditures");

            office.Balance -= model.Amount;

            var expenditure = new Expenditure
            {
                Name = model.Name,
                Amount = model.Amount,
                DeskGroupId = model.SelectedDeskGroupId,
                TypeId = model.SelectedTypeId,
                ObjectId = model.SelectedObjectId,
                PaymentTypeId = PaymentTypes.Cash
            };


            var operation = new ExpenditureOperation
            {
                Expenditure = expenditure,
                OperationDateTime = DateTime.Now,
                OperationTypeId = ExpenditureOperation.EOType.New
            };

            _db.ExpenditureOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("IssuedExpenditures");
        }

        [HttpPost]
        public async Task<ActionResult> CancelExpenditure(int id)
        {
            var expenditure = await _db.Expenditures.FindAsync(id);
            if (expenditure == null) return PartialView("Error", new[] { "Операция не найдена" });

            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("IssuedExpenditures");

            office.Balance += expenditure.Amount;

            var operation = new ExpenditureOperation
            {
                Expenditure = expenditure,
                OperationDateTime = DateTime.Now,
                OperationTypeId = ExpenditureOperation.EOType.Cancelled
            };

            _db.ExpenditureOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("IssuedExpenditures");
        }

        [HttpGet]
        public ActionResult IncomingExpenditures()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            return PartialView(new IncomingExpendituresViewModel
            {
                Items = (from e in _db.IncomingExpenditures.Include(e => e.FinancialAccountOperation)
                            .ThenInclude(fao => fao.Counterparty)
                         orderby e.FinancialAccountOperation.InsertDateTime descending
                         select new IncomingExpenditureItem
                         {
                             Amount = e.Amount.Value.ToString("#,0.00", nfi),
                             Description = e.FinancialAccountOperation.Description,
                             OperationId = e.IncomingExpenditureId,
                             CreatedDateTime = e.FinancialAccountOperation.OperationDateTime,
                             CounterpartyName = e.FinancialAccountOperation.Counterparty.Name,
                             IsProcessed = e.IsProcessed
                         }).ToList()
            });
        }

        [HttpGet]
        public ActionResult CashlessExpenditures()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var expenditureList = (from expenditure in _db.Expenditures
                                    .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                                    .Where(e => e.PaymentTypeId == PaymentTypes.Cashless)
                                   join eo in _db.ExpenditureOperations on expenditure.ExpenditureId equals eo.ExpenditureId into operations
                                   from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                                   orderby operation.OperationDateTime descending
                                   select new { expenditure, operation }).ToList();

            var model = new ExpendituresViewModel
            {
                Items = (from e in expenditureList
                         select new ExpenditureViewItem
                         {
                             ExpenditureId = e.expenditure.ExpenditureId,
                             Amount = e.expenditure.Amount.ToString("#,0.00", nfi),
                             Name = e.expenditure.Name,
                             DeskGroup = e.expenditure.DeskGroup != null ? e.expenditure.DeskGroup.Name : "",
                             Type = e.expenditure.Type != null ? e.expenditure.Type.Description : "",
                             Object = e.expenditure.Object != null ? e.expenditure.Object.Description : "",
                             IssuedDateTime = e.operation.OperationDateTime,
                             Status = e.operation.OperationTypeId
                         }).ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ProcessIncomingExpenditure(int id)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new ProcessIncomingExpenditureViewModel
            {
                Expenditure = _db.IncomingExpenditures.FirstOrDefault(e => e.IncomingExpenditureId == id),
                DeskGroups = (from d in _db.DeskGroups
                              where d.IsActive
                              select new KeyValuePair<int,string>(d.DeskGroupId, d.Name)).ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ProcessIncomingExpenditure([FromBody]ProcessIncomingExpenditurePostModel model)
        {
            var incomingExpenditure = _db.IncomingExpenditures.Include(e => e.FinancialAccountOperation)
                .ThenInclude(fao => fao.Counterparty)
                .FirstOrDefault(e => e.IncomingExpenditureId == model.ExpenditureId);

            foreach (var item in model.Items)
            {
                var deskGroup = _db.DeskGroups.FirstOrDefault(g => g.DeskGroupId == item.GroupId);

                var expenditure = new Expenditure
                {
                    Name = deskGroup.Name,
                    Amount = item.Amount,
                    DeskGroupId = item.GroupId,
                    //TypeId = model.SelectedTypeId,
                    //ObjectId = model.SelectedObjectId,
                    PaymentTypeId = PaymentTypes.Cashless
                };

                var operation = new ExpenditureOperation
                {
                    Expenditure = expenditure,
                    OperationDateTime = DateTime.Now,
                    OperationTypeId = ExpenditureOperation.EOType.New
                };

                _db.ExpenditureOperations.Add(operation);
            }

            incomingExpenditure.IsProcessed = true;

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public ActionResult ProcessIncomingExpenditures()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new ProcessIncomingExpenditureViewModel
            {
                DeskGroups = (from d in _db.DeskGroups
                              where d.IsActive
                              select new KeyValuePair<int, string>(d.DeskGroupId, d.Name)).ToList()
            };

            return PartialView(model);
        }
    }
}