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
using AvibaWeb.Infrastructure;
using AvibaWeb.ViewModels.LoanExpenditureViewModels;
using Microsoft.AspNetCore.Identity;

namespace AvibaWeb.Controllers
{
    public class ExpenditureController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private const int PageSize = 10;

        public ExpenditureController(AppIdentityDbContext db, UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
        }

        // GET: Expenditures
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult IssuedExpenditures(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var expenditureList = (from expenditure in _db.Expenditures
                    .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                    .Where(e => e.PaymentTypeId == PaymentTypes.Cash)
                join eo in _db.ExpenditureOperations on expenditure.ExpenditureId equals eo.ExpenditureId into operations
                from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                orderby operation.OperationDateTime descending
                where operation.OperationDateTime >= queryFromDate && operation.OperationDateTime < queryToDate.AddDays(1)
                select new { expenditure, operation }).ToList();

            var model = new ExpendituresViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
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
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("IssuedExpenditures");

            var otherSum = decimal.Parse(_db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value
                .Replace(" ", string.Empty), CultureInfo.InvariantCulture);
            var remainder = model.Amount;
            if (otherSum > 0)
            {
                _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value = otherSum >= model.Amount
                    ? (otherSum - model.Amount).ToString("#,0", nfi)
                    : (0).ToString("#,0", nfi);
            }

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

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.ExpenditureOperations.Add(operation);
                _db.SetUserContext(user.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

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

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.ExpenditureOperations.Add(operation);
                _db.SetUserContext(user.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

            return RedirectToAction("IssuedExpenditures");
        }

        [HttpGet]
        public ActionResult IncomingExpenditures(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            return PartialView(new IncomingExpendituresViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                Items = (from e in _db.IncomingExpenditures.Include(e => e.FinancialAccountOperation)
                            .ThenInclude(fao => fao.Counterparty)
                         orderby e.IsProcessed, e.FinancialAccountOperation.OperationDateTime descending
                         where e.FinancialAccountOperation.OperationDateTime >= queryFromDate &&
                               e.FinancialAccountOperation.OperationDateTime < queryToDate.AddDays(1)
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
        public ActionResult CashlessExpenditures(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var expenditureList = (from expenditure in _db.Expenditures
                    .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                    .Include(e => e.IncomingExpenditure).ThenInclude(ie => ie.FinancialAccountOperation)
                    .Where(e => e.PaymentTypeId == PaymentTypes.Cashless)
                from eo in _db.ExpenditureOperations.Where(eo => expenditure.ExpenditureId == eo.ExpenditureId)
                    .OrderByDescending(eo => eo.OperationDateTime).Take(1).DefaultIfEmpty()
                    //join eo in _db.ExpenditureOperations.Where(ieo => ieo.OperationDateTime >= queryFromDate && ieo.OperationDateTime < queryToDate) on expenditure.ExpenditureId equals eo.ExpenditureId into operations
                    //from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                    //orderby operation.OperationDateTime descending
                                   where eo.OperationDateTime >= queryFromDate && eo.OperationDateTime < queryToDate.AddDays(1)
                                   orderby eo.Expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime descending
                                   select new {expenditure, eo}).ToList()
                                   .GroupBy(f => f.expenditure.IncomingExpenditureId, new NullableComparer<int>());

            var model = new CashlessExpendituresViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                ItemGroups = (from e in expenditureList
                              select new ExpenditureViewItemGroup
                              {
                                  Status = e.FirstOrDefault().eo.OperationTypeId,
                                  IncomingExpenditureId = e.FirstOrDefault().expenditure.IncomingExpenditureId,
                                  Description = e.FirstOrDefault().expenditure.IncomingExpenditure?.FinancialAccountOperation.Description,
                                  Items = (from g in e
                                           select new ExpenditureViewItem
                                           {
                                               ExpenditureId = g.expenditure.ExpenditureId,
                                               Amount = g.expenditure.Amount.ToString("#,0.00", nfi),
                                               Name = g.expenditure.Name,
                                               DeskGroup = g.expenditure.DeskGroup != null ? g.expenditure.DeskGroup.Name : "",
                                               Type = g.expenditure.Type != null ? g.expenditure.Type.Description : "",
                                               Object = g.expenditure.Object != null ? g.expenditure.Object.Description : "",
                                               IssuedDateTime = g.expenditure.IncomingExpenditure?.FinancialAccountOperation.OperationDateTime ?? g.eo.OperationDateTime,
                                               Status = g.eo.OperationTypeId
                                           }).ToList()
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
                ExpenditureObjects = new SelectList(
                    (from o in _db.ExpenditureObjects.Where(o => o.IsActive).OrderBy(o => o.Description)
                     select new
                     {
                         Id = o.ExpenditureObjectId,
                         Name = o.Description
                     }).ToList(),
                    "Id",
                    "Name"),
                DeskGroups = (from d in _db.DeskGroups
                              where d.IsActive
                              select new KeyValuePair<int, string>(d.DeskGroupId, d.Name)).ToList()
            };

            var counterparty = (from c in _db.Counterparties
                                join fao in _db.FinancialAccountOperations on c.ITN equals fao.CounterpartyId
                                where fao.FinancialAccountOperationId == model.Expenditure.FinancialAccountOperationId
                                select c).FirstOrDefault();

            if (counterparty != null)
            {
                if (counterparty.ExpenditureObjectId != null)
                {
                    model.ExpenditureObjects.FirstOrDefault(e => e.Value == counterparty.ExpenditureObjectId.ToString()).Selected = true;
                }
            }

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> ProcessIncomingExpenditure([FromBody]ProcessIncomingExpenditurePostModel model)
        {
            var incomingExpenditure = _db.IncomingExpenditures.Include(e => e.FinancialAccountOperation)
                .ThenInclude(fao => fao.Counterparty)
                .Include(e => e.FinancialAccountOperation.PayeeUser)
                .FirstOrDefault(e => e.IncomingExpenditureId == model.ExpenditureId);

            foreach (var item in model.Items)
            {
                var deskGroup = _db.DeskGroups.FirstOrDefault(g => g.DeskGroupId == item.GroupId);

                var expenditure = new Expenditure
                {
                    Name = incomingExpenditure.FinancialAccountOperation.Counterparty?.Name ??
                           incomingExpenditure.FinancialAccountOperation.PayeeUser?.Name ?? "" + " - " +
                           incomingExpenditure.FinancialAccountOperation.Description,
                    Amount = item.Amount,
                    DeskGroupId = item.GroupId,
                    TypeId = _db.ExpenditureTypes.FirstOrDefault(et => et.Description == "Расход").ExpenditureTypeId,
                    ObjectId = model.ExpenditureObjectId,
                    PaymentTypeId = PaymentTypes.Cashless,
                    IncomingExpenditure = incomingExpenditure
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

        [HttpPost]
        public async Task<ActionResult> CancelExpenditureGroup(int id)
        {
            List<Expenditure> expenditureList = (from e in _db.Expenditures.Include(e => e.IncomingExpenditure)
                                                 where e.IncomingExpenditureId == id
                                                 select e).ToList();

            foreach (Expenditure e in expenditureList)
            {
                var operation = new ExpenditureOperation
                {
                    Expenditure = e,
                    OperationDateTime = DateTime.Now,
                    OperationTypeId = ExpenditureOperation.EOType.Cancelled
                };

                _db.ExpenditureOperations.Add(operation);
            }

            if (expenditureList.Count() > 0)
            {
                expenditureList.FirstOrDefault().IncomingExpenditure.IsProcessed = false;
            }

            await _db.SaveChangesAsync();

            return RedirectToAction("CashlessExpenditures");
        }

        [HttpGet]
        public ActionResult CreateLoanExpenditure()
        {
            var model = new CreateLoanExpenditureModel()
            {
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateLoanExpenditure(CreateLoanExpenditureModel model)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("LoanExpenditures");

            var otherSum = decimal.Parse(_db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value
                .Replace(" ", string.Empty), CultureInfo.InvariantCulture);
            var remainder = model.Amount;
            if (otherSum > 0)
            {
                _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value = otherSum >= model.Amount
                    ? (otherSum - model.Amount).ToString("#,0", nfi)
                    : (0).ToString("#,0", nfi);
            }

            office.Balance -= model.Amount;

            var expenditure = new LoanExpenditure
            {
                Description = model.Name,
                Amount = model.Amount
            };

            var operation = new LoanExpenditureOperation
            {
                LoanExpenditure = expenditure,
                OperationDateTime = DateTime.Now,
                OperationTypeId = LoanExpenditureOperation.LEOType.New
            };

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.LoanExpenditureOperations.Add(operation);
                _db.SetUserContext(user.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

            return RedirectToAction("LoanExpenditures");
        }

        [HttpPost]
        public async Task<ActionResult> CancelLoanExpenditure(int id)
        {
            var expenditure = await _db.LoanExpenditures.FindAsync(id);
            if (expenditure == null) return PartialView("Error", new[] { "Операция не найдена" });

            var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            if (office == null) return RedirectToAction("LoanExpenditures");

            office.Balance += expenditure.Amount;

            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.LoanExpenditures.Remove(expenditure);
                _db.SetUserContext(user.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

            return RedirectToAction("LoanExpenditures");
        }

        [HttpGet]
        public ActionResult LoanExpenditures(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var expenditureList = (from expenditure in _db.LoanExpenditures
                                   join eo in _db.LoanExpenditureOperations on expenditure.LoanExpenditureId equals eo.LoanExpenditureId into operations
                                   from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                                   orderby operation.OperationDateTime descending
                                   where operation.OperationDateTime >= queryFromDate && operation.OperationDateTime < queryToDate.AddDays(1)
                                   select new { expenditure, operation }).ToList();

            var model = new LoanExpendituresViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                Items = (from e in expenditureList
                         select new LoanExpenditureViewItem
                         {
                             LoanExpenditureId = e.expenditure.LoanExpenditureId,
                             Amount = e.expenditure.Amount.ToString("#,0.00", nfi),
                             Description = e.expenditure.Description,
                             IssuedDateTime = e.operation.OperationDateTime,
                             Status = e.operation.OperationTypeId
                         }).ToList()
            };

            return PartialView(model);
        }
    }
}