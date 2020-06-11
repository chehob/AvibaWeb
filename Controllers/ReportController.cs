using System;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.ReportViewModels;
using Microsoft.CodeAnalysis;
using System.Globalization;
using AvibaWeb.DomainModels;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using AvibaWeb.ViewModels.ExpenditureViewModels;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace AvibaWeb.Controllers
{
    public class ReportController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;

        public ReportController(AppIdentityDbContext db, UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
        }

        // GET: Report
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult OnlineInfo()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var isUserAdmin = User.IsInRole("Administrators");
            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));
            var collectorRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Инкассаторы"));
            var avibaDeskFilter = new[] { "ГРБЕ53", "ГРБЕ54", "ГРБЕ55", "ГРБЕ21", "ГРБЕ22", "ГРБЕ35", "ГРБЕ33" };
            var aviaTourDeskFilter = new[] { "ГРБЕ56", "ГРБЕ12", "ГРБЕ58", "ГРБЕ57", "ГРБЕ59", "ГРБЕ16", "ГРБЕ61", "ГРБЕ60", "ГРБЕ41" };
            var model = new OnlineInfoModel
            {
                AvibaBalanceInfo = (from v in _db.VDeskBalances
                    where avibaDeskFilter.Contains(v.DeskId) && v.Balance != 0
                    orderby v.DeskName
                    select new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = v.DeskName,
                        Balance = v.Balance
                    }).ToList(),
                AviaTourBalanceInfo = (from v in _db.VDeskBalances
                    where aviaTourDeskFilter.Contains(v.DeskId) && v.Balance != 0
                    orderby v.DeskName
                    select new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = v.DeskName,
                        Balance = v.Balance
                    }).ToList(),
                CollectorsBalanceInfo = _db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == collectorRole.Id && u.Balance != 0) &&
                                u.Roles.All(r => r.RoleId != officeRole.Id) &&
                                (u.Name != "Инкассатор" || (u.Name == "Инкассатор" && isUserAdmin)) )
                    .Select(u => new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = u.Name,
                        Balance = u.Balance
                    }).ToList(),
                LoanBalanceInfo =
                    (from expenditure in _db.LoanExpenditures
                        join eo in _db.LoanExpenditureOperations on expenditure.LoanExpenditureId equals eo.LoanExpenditureId into operations
                        from operation in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                        orderby operation.OperationDateTime descending
                        where operation.OperationTypeId == LoanExpenditureOperation.LEOType.New
                        select expenditure)
                    .Select(u => new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = u.Description,
                        Balance = u.Amount
                    }).ToList(),
                OfficeBalance = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).Sum(u => u.Balance),
                TransitBalance = _db.TransitAccounts.FirstOrDefault().Balance
            };

            model.OfficeBillInfo = new OfficeBillInfoViewModel
            {
                _5kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "5kBillSum").Value,
                _2kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum").Value,
                OtherSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value
            };

            model.OfficeBillInfo.RemainderSum = (model.OfficeBalance -
                                                 decimal.Parse(model.OfficeBillInfo._5kBillSum.Replace(".", ",")
                                                     .Replace(" ", string.Empty)) -
                                                 decimal.Parse(model.OfficeBillInfo._2kBillSum.Replace(".", ",")
                                                     .Replace(" ", string.Empty)) -
                                                 decimal.Parse(model.OfficeBillInfo.OtherSum.Replace(".", ",")
                                                     .Replace(" ", string.Empty)))
                .ToString("#,0", nfi);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult CashlessOrg()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var reportDate = DateTime.Today.Date;

            var model = new OrganizationCashlessViewModel
            {
                Organizations = (from o in _db.Organizations.Where(io => io.IsActive)
                    join fa in _db.FinancialAccounts.Where(ifa => ifa.IsActive) on o.OrganizationId equals fa.OrganizationId
                    join fao in _db.FinancialAccountOperations on fa.FinancialAccountId equals fao.FinancialAccountId into
                        operations
                    group new { o, fa, operations = operations.DefaultIfEmpty() } by o.OrganizationId
                    into g
                    select new OrganizationCashlessInfo
                    {                        
                        Name = g.FirstOrDefault().o.Description,
                        AccountBalances = (from o in g.SelectMany(q => q.operations)
                                           group o by o.FinancialAccountId
                                            into og
                                           join fa in _db.FinancialAccounts on og.Key equals fa.FinancialAccountId
                                           //let UploadDate = og.OrderByDescending(o => o.InsertDateTime).FirstOrDefault().InsertDateTime
                                           let UploadDate = fa.LastUploadDate == null ? "Нет даты" : fa.LastUploadDate.Value.Date.Equals(DateTime.Now.Date) ? fa.LastUploadDate.Value.ToString("t") : fa.LastUploadDate.Value.ToString("dd.MM hh:mm")
                                           select new OrganizationAccountBalance
                                           {
                                               Id = fa.FinancialAccountId,
                                               //LatestUpload = UploadDate.Date.Equals(DateTime.Now.Date) ? UploadDate.ToString("t") : UploadDate.ToString("dd.MM hh:mm"),
                                               LatestUpload = UploadDate,
                                               Account = fa.Description,
                                               BankName = fa.BankName,
                                               Balance = fa.Balance.ToString("#,0.00", nfi)
                                           }).ToList()
                    }).ToList(),
                LoanGroupsBalance = _db.LoanGroups.Where(lg => lg.Description != "Дивиденты").Sum(lg => lg.Balance).ToString("#,0.00", nfi)
            };

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult CashlessCounterparties()
        {
            var model = new CashlessCorpViewModel
            {
                //Items = (from v in _db.VCorpBalances
                //         where v.Balance != 0
                //         orderby v.Balance                      
                //         select new CashlessCorpItem
                //         {
                //             CorpName = v.CorpName,
                //             Balance = v.Balance,
                //             LastPaymentDays = v.LastPaymentDate == null ? 0 : (DateTime.Now - v.LastPaymentDate).Days,
                //             LastReceiptDays = v.LastReceiptDate == null ? 0 : BusinessDaysUntil(v.LastReceiptDate, DateTime.Now)
                //         }).ToList()
                Items = (from v in _db.CorporatorAccounts
                         join c in _db.Counterparties.Include(c => c.Type) on v.ITN equals c.ITN
                         where v.Balance != 0
                         orderby v.Balance
                         where c.Type.Description == "Корпоратор" && c.LoanGroupId == null
                         select new CashlessCorpItem
                         {
                             CorpName = v.Corporator.Name,
                             Balance = v.Balance,
                             LastPaymentDays = v.LastPaymentDate == null ? 0 : (DateTime.Now - v.LastPaymentDate).Value.Days,
                             LastReceiptDays = v.LastReceiptDate == null ? 0 : BusinessDaysUntil(v.LastReceiptDate.Value, DateTime.Now)
                         }).ToList()
            };

            return PartialView(model);
        }

        private int BusinessDaysUntil(DateTime firstDay, DateTime lastDay, params DateTime[] bankHolidays)
        {
            firstDay = firstDay.Date;
            lastDay = lastDay.Date;
            if (firstDay > lastDay)
                throw new ArgumentException("Incorrect last day " + lastDay);

            TimeSpan span = lastDay - firstDay;
            int businessDays = span.Days + 1;
            int fullWeekCount = businessDays / 7;
            // find out if there are weekends during the time exceedng the full weeks
            if (businessDays > fullWeekCount * 7)
            {
                // we are here to find out if there is a 1-day or 2-days weekend
                // in the time interval remaining after subtracting the complete weeks
                int firstDayOfWeek = firstDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)firstDay.DayOfWeek;
                int lastDayOfWeek = lastDay.DayOfWeek == DayOfWeek.Sunday ? 7 : (int)lastDay.DayOfWeek;
                if (lastDayOfWeek < firstDayOfWeek)
                    lastDayOfWeek += 7;
                if (firstDayOfWeek <= 6)
                {
                    if (lastDayOfWeek >= 7)// Both Saturday and Sunday are in the remaining time interval
                        businessDays -= 2;
                    else if (lastDayOfWeek >= 6)// Only Saturday is in the remaining time interval
                        businessDays -= 1;
                }
                else if (firstDayOfWeek <= 7 && lastDayOfWeek >= 7)// Only Sunday is in the remaining time interval
                    businessDays -= 1;
            }

            // subtract the weekends during the full weeks in the interval
            businessDays -= fullWeekCount + fullWeekCount;

            // subtract the number of bank holidays during the time interval
            foreach (DateTime bankHoliday in bankHolidays)
            {
                DateTime bh = bankHoliday.Date;
                if (firstDay <= bh && bh <= lastDay)
                    --businessDays;
            }

            return businessDays;
        }

        [HttpGet]
        public ActionResult OfficeBillEdit()
        {
            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));

            var model = new OfficeBillEditViewModel
            {
                _5kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "5kBillSum").Value,
                _2kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum").Value,
                OtherSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value,
                OfficeBalance = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).Sum(u => u.Balance)
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> OfficeBillEdit(OfficeBillEditViewModel model)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var settingsValue = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "5kBillSum");
            settingsValue.Value = decimal.Parse(model._5kBillSum.Replace(".", ",").Replace(" ", string.Empty)).ToString("#,0", nfi);

            settingsValue = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum");
            settingsValue.Value = decimal.Parse(model._2kBillSum.Replace(".", ",").Replace(" ", string.Empty)).ToString("#,0", nfi);

            settingsValue = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum");
            settingsValue.Value = decimal.Parse(model.OtherSum.Replace(".", ",").Replace(" ", string.Empty)).ToString("#,0", nfi);

            await _db.SaveChangesAsync();

            return RedirectToAction("OfficeBillInfo");
        }

        [HttpGet]
        public ActionResult OfficeBillInfo()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new OfficeBillInfoViewModel
            {
                _5kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "5kBillSum").Value,
                _2kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum").Value,
                OtherSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value,
            };

            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));

            model.RemainderSum =
                (_db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).Sum(u => u.Balance) -
                 decimal.Parse(model._5kBillSum.Replace(".", ",").Replace(" ", string.Empty)) -
                 decimal.Parse(model._2kBillSum.Replace(".", ",").Replace(" ", string.Empty)) -
                 decimal.Parse(model.OtherSum.Replace(".", ",").Replace(" ", string.Empty)))
                .ToString("#,0", nfi);

            return PartialView(model);
        }        

        [HttpGet]
        public ActionResult AccountOperations(DateTime? fromDate, DateTime? toDate, int accountId = 0)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var model = (from fa in _db.FinancialAccounts
                where fa.FinancialAccountId == accountId || accountId == 0
                select new AccountOperationsViewModel
                {
                    AccountId = accountId,
                    FromDate = queryFromDate.ToString("d"),
                    ToDate = queryToDate.ToString("d"),
                    IsAllOperations = accountId == 0,
                    OrgName = accountId == 0 ? "Все" : fa.Organization.Description,
                    BankName = accountId == 0 ? "" : fa.BankName,
                    Operations =
                        (from fao in _db.FinancialAccountOperations.Include(fao => fao.Account).ThenInclude(a => a.Organization)
                            where (fao.FinancialAccountId == accountId || accountId == 0) &&
                                fao.OperationDateTime.Date >= queryFromDate && fao.OperationDateTime.Date <= queryToDate
                            orderby fao.OperationDateTime descending
                            select new AccountOperationData
                            {
                                OperationDateTime = fao.OperationDateTime.ToString("d"),
                                OrderNumber = fao.OrderNumber,
                                CounterpartyName =
                                    fao.TransferAccount != null
                                        ? fao.TransferAccount.Organization.Description + " - " + fao.TransferAccount.BankName
                                        : (fao.Counterparty != null
                                            ? fao.Counterparty.Name
                                            : (fao.PayeeUser != null ? fao.PayeeUser.Name : "")),
                                PayeeName = fao.Account.Organization.Description + " - " + fao.Account.BankName,
                                Amount = fao.Amount,
                                AmountStr = fao.Amount.ToString("#,0.00", nfi),
                                Description = fao.Description
                            }).ToList()
                }).FirstOrDefault();
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ExpenditureSummary(ExpenditureSummaryGrouping grouping, DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var model = new ExpenditureSummaryViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                ItemGroups = (from expenditure in _db.Expenditures
                        .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                        .Include(e => e.IncomingExpenditure).ThenInclude(ie => ie.FinancialAccountOperation)
                    from eo in _db.ExpenditureOperations.Where(eo => expenditure.ExpenditureId == eo.ExpenditureId)
                        .OrderByDescending(eo => eo.OperationDateTime).Take(1).DefaultIfEmpty()
                    where ((expenditure.PaymentTypeId == PaymentTypes.Cash && eo.OperationDateTime >= queryFromDate &&
                            eo.OperationDateTime < queryToDate.AddDays(1)) ||
                           (expenditure.PaymentTypeId == PaymentTypes.Cashless &&
                            expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime >=
                            queryFromDate &&
                            expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime <
                            queryToDate.AddDays(1))) &&
                          eo.OperationTypeId == ExpenditureOperation.EOType.New
                    group expenditure by new
                    {
                        groupField =
                            (grouping == ExpenditureSummaryGrouping.ByDeskGroup
                                ? expenditure.DeskGroupId
                                : expenditure.ObjectId)
                    }
                    into g
                    select new ExpenditureSummaryViewItemGroup
                    {
                        Name = (grouping == ExpenditureSummaryGrouping.ByDeskGroup
                            ? g.FirstOrDefault().DeskGroup.Name
                            : g.FirstOrDefault().Object.Description),
                        AmountCash = g.Where(ig => ig.PaymentTypeId == PaymentTypes.Cash).Sum(ig => ig.Amount),
                        AmountCashless = g.Where(ig => ig.PaymentTypeId == PaymentTypes.Cashless).Sum(ig => ig.Amount),
                        Items = (from item in g
                            group item by new
                            {
                                groupField =
                                    (grouping == ExpenditureSummaryGrouping.ByDeskGroup
                                        ? item.ObjectId
                                        : item.DeskGroupId)
                            }
                            into sg
                            select new ExpenditureSummaryViewItem
                            {
                                AmountCash =
                                    sg.Where(isg => isg.PaymentTypeId == PaymentTypes.Cash).Sum(isg => isg.Amount),
                                AmountCashless = sg.Where(isg => isg.PaymentTypeId == PaymentTypes.Cashless)
                                    .Sum(isg => isg.Amount),
                                Name = (grouping == ExpenditureSummaryGrouping.ByDeskGroup
                                    ? sg.FirstOrDefault().Object.Description
                                    : sg.FirstOrDefault().DeskGroup.Name),
                                DeskGroupId = sg.FirstOrDefault().DeskGroupId,
                                ObjectId = sg.FirstOrDefault().ObjectId
                            }).ToList()
                    }).ToList()
            };
            model.AmountCash = model.ItemGroups.Sum(ig => ig.AmountCash).ToString("#,0.00", nfi);
            model.AmountCashless = model.ItemGroups.Sum(ig => ig.AmountCashless).ToString("#,0.00", nfi);
            model.Amount = model.ItemGroups.Sum(ig => ig.Amount).ToString("#,0.00", nfi);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult ExpenditureSummaryOperations(int deskGroupId, int? objectId, DateTime fromDate, DateTime toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new ExpenditureSummaryOperationsViewModel
            {
                Items = (from expenditure in _db.Expenditures
                        .Include(e => e.DeskGroup).Include(e => e.Type).Include(e => e.Object)
                        .Include(e => e.IncomingExpenditure).ThenInclude(ie => ie.FinancialAccountOperation).ThenInclude(fao => fao.Counterparty)
                    from eo in _db.ExpenditureOperations.Where(eo => expenditure.ExpenditureId == eo.ExpenditureId)
                        .OrderByDescending(eo => eo.OperationDateTime).Take(1).DefaultIfEmpty()
                    where ((expenditure.PaymentTypeId == PaymentTypes.Cash && eo.OperationDateTime >= fromDate &&
                            eo.OperationDateTime < toDate.AddDays(1)) ||
                           (expenditure.PaymentTypeId == PaymentTypes.Cashless &&
                            expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime >=
                            fromDate &&
                            expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime <
                            toDate.AddDays(1))) &&
                          eo.OperationTypeId == ExpenditureOperation.EOType.New &&
                          expenditure.DeskGroupId == deskGroupId && (objectId == null || expenditure.ObjectId == objectId)
                    select new ExpenditureSummaryOperationsItem
                    {
                        OperationDateTime = expenditure.PaymentTypeId == PaymentTypes.Cash ? eo.OperationDateTime.ToString("dd.MM.yyyy HH:mm") : expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime.ToString("dd.MM.yyyy HH:mm"),
                        Amount = expenditure.Amount.ToString("#,0.00", nfi),
                        PaymentType = expenditure.PaymentTypeId == PaymentTypes.Cash ? "Нал" : "Безнал",
                        Counterparty = expenditure.PaymentTypeId == PaymentTypes.Cash ? "" : expenditure.IncomingExpenditure.FinancialAccountOperation.Counterparty.Name,
                        Comment = expenditure.PaymentTypeId == PaymentTypes.Cash ? expenditure.Name : expenditure.IncomingExpenditure.FinancialAccountOperation.Description
                    }).ToList()
            };

            return PartialView(model);
        }
        
        [HttpGet]
        public ActionResult IncomeSummary(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate.AddDays(-30);

            var deskGroupList = (from dg in _db.DeskGroups
                select new
                {
                    Name = dg.Name
                }).ToList();

            var KRSList = (from dg in _db.DeskGroups
                           join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                           join info in _db.VServiceReceiptIncomeInfo on d.DeskId equals info.DeskIssuedId
                           where info.DateTime >= queryFromDate && info.DateTime < queryToDate.AddDays(1)
                           group new { info, dg } by dg.DeskGroupId into g
                           select new IncomeSummaryViewItemGroup
                           {
                               Name = g.FirstOrDefault().dg.Name,
                               AmountKRS = g.Sum(ig => ig.info.Amount)
                           }).ToList();

            var CorpTicketList = (from dg in _db.DeskGroups
                     join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                     join ti in _db.VReceiptTicketInfo on d.DeskId equals ti.DeskId
                     join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                     join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                     where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) &&
                        cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                        cr.TypeId == CorporatorReceipt.CRType.CorpClient && cri.TypeId == CorporatorReceiptItem.CRIType.Ticket
                     group new { dg, cri, ti } by dg.DeskGroupId into g
                     select new IncomeSummaryViewItemGroup
                     {
                         Name = g.FirstOrDefault().dg.Name,
                         AmountCorp = g.Sum(sg => sg.cri.IsPercent ? sg.cri.Amount * sg.cri.FeeRate / 100 : sg.cri.PerSegment ? sg.cri.FeeRate * sg.ti.SegCount : sg.cri.FeeRate)
                     }).ToList();

            var CorpLuggageList = (from dg in _db.DeskGroups
                                  join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                                  join ti in _db.VReceiptLuggageInfo on d.DeskId equals ti.DeskId
                                  join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                                  join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                                  where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) && 
                                    cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                                    cr.TypeId == CorporatorReceipt.CRType.CorpClient && cri.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                  group new { dg, cri, ti } by dg.DeskGroupId into g
                                  select new IncomeSummaryViewItemGroup
                                  {
                                      Name = g.FirstOrDefault().dg.Name,
                                      AmountCorp = g.Sum(sg => sg.cri.IsPercent ? sg.cri.Amount * sg.cri.FeeRate / 100 : sg.cri.FeeRate),
                                  }).ToList();

            var OtherList = (from income in _db.Incomes
                    .Include(e => e.DeskGroup)
                from io in _db.IncomeOperations.Where(io => income.IncomeId == io.IncomeId)
                    .OrderByDescending(io => io.OperationDateTime).Take(1).DefaultIfEmpty()
                where io.OperationDateTime >= queryFromDate &&
                      io.OperationDateTime < queryToDate.AddDays(1) &&
                      io.OperationTypeId == IncomeOperation.IOType.New
                group income by income.DeskGroupId
                into g
                select new
                {
                    Name = g.FirstOrDefault().DeskGroup.Name,
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var model = new IncomeSummaryViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                ItemGroups = (from dg in deskGroupList
                              from k in KRSList.Where(c => c.Name == dg.Name).DefaultIfEmpty()
                              from ct in CorpTicketList.Where(c => c.Name == dg.Name).DefaultIfEmpty()
                              from cl in CorpLuggageList.Where(c => c.Name == dg.Name).DefaultIfEmpty()
                              from ol in OtherList.Where(c => c.Name == dg.Name).DefaultIfEmpty()
                              select new IncomeSummaryViewItemGroup
                              {
                                  Name = dg.Name,
                                  AmountKRS = (k == null ? 0 : k.AmountKRS),
                                  AmountCorp = (ct == null ? 0 : ct.AmountCorp) + (cl == null ? 0 : cl.AmountCorp),
                                  AmountOther = ol?.Amount ?? 0
                              }).ToList()
            };
            model.AmountKRS = model.ItemGroups.Sum(ig => ig.AmountKRS).ToString("#,0.00", nfi);
            model.AmountCorp = model.ItemGroups.Sum(ig => ig.AmountCorp).ToString("#,0.00", nfi);
            model.AmountAgentFee = (from c in _db.Counterparties.Where(c => c.Type.Description == "Провайдер услуг")
                                    from t in _db.ProviderAgentFeeTransactions
                                        .Where(t => t.TransactionDateTime >= queryFromDate &&
                                            t.TransactionDateTime < queryToDate.AddDays(1) && c.ITN == t.ProviderId).DefaultIfEmpty()
                                    select t).Sum(t => t.Amount).ToString("#,0.00", nfi);
            model.AmountSubagent = (from s in _db.SubagentFeeTransactions
                                    where s.TransactionDateTime >= queryFromDate && s.TransactionDateTime < queryToDate.AddDays(1)
                                    select s).Sum(s => s.Amount).ToString("#,0.00", nfi);
            model.AmountOther = model.ItemGroups.Sum(ig => ig.AmountOther).ToString("#,0.00", nfi);

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult FinalSummary(DateTime? fromDate, DateTime? toDate)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var today = DateTime.Today;
            var currentMonth = new DateTime(today.Year, today.Month, 1);
            var queryToDate = toDate ?? currentMonth.AddDays(-1);
            var queryFromDate = fromDate ?? currentMonth.AddMonths(-1);

            var customIncomeList = (from cif in _db.VCustomIncomeInfo
                    where cif.OperationDateTime >= queryFromDate && cif.OperationDateTime < queryToDate.AddDays(1)
                    select new
                    {
                        cif.GroupId,
                        cif.Amount
                    }).GroupBy(ig => ig.GroupId)
                .Select(g => new
                {
                    g.Key,
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var KRSList = (from dg in _db.DeskGroups
                           join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                           join info in _db.VServiceReceiptIncomeInfo on d.DeskId equals info.DeskIssuedId
                           where info.DateTime >= queryFromDate && info.DateTime < queryToDate.AddDays(1)
                           select new
                           {
                               dg.DeskGroupId,
                               info.Amount
                           }).GroupBy(ig => ig.DeskGroupId)
                           .Select(g => new
                           {
                               g.Key,
                               Amount = g.Sum(ig => ig.Amount)
                           }).ToList();

            var CorpTicketList = (from dg in _db.DeskGroups
                                  join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                                  join ti in _db.VReceiptTicketInfo on d.DeskId equals ti.DeskId
                                  join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                                  join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                                  where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) &&
                                     cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                                     cr.TypeId == CorporatorReceipt.CRType.CorpClient && cri.TypeId == CorporatorReceiptItem.CRIType.Ticket
                                  select new
                                  {
                                      dg.DeskGroupId,
                                      Amount = cri.IsPercent ? cri.Amount * cri.FeeRate / 100 : cri.PerSegment ? cri.FeeRate * ti.SegCount : cri.FeeRate
                                  }).GroupBy(ig => ig.DeskGroupId)
                                  .Select(g => new
                                  {
                                      g.Key,
                                      Amount = g.Sum(ig => ig.Amount)
                                  }).ToList();

            var CorpLuggageList = (from dg in _db.DeskGroups
                                   join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                                   join ti in _db.VReceiptLuggageInfo on d.DeskId equals ti.DeskId
                                   join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                                   join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                                   where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) &&
                                     cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                                     cr.TypeId == CorporatorReceipt.CRType.CorpClient && cri.TypeId == CorporatorReceiptItem.CRIType.Luggage
                                   select new
                                   {
                                       dg.DeskGroupId,
                                       Amount = cri.IsPercent ? cri.Amount * cri.FeeRate / 100 : cri.FeeRate
                                   }).GroupBy(ig => ig.DeskGroupId)
                                   .Select(g => new
                                   {
                                       g.Key,
                                       Amount = g.Sum(ig => ig.Amount)
                                   }).ToList();

            var otherIncomeList = (from income in _db.Incomes
                    .Include(e => e.DeskGroup)
                from io in _db.IncomeOperations.Where(io => income.IncomeId == io.IncomeId)
                    .OrderByDescending(io => io.OperationDateTime).Take(1).DefaultIfEmpty()
                where io.OperationDateTime >= queryFromDate &&
                      io.OperationDateTime < queryToDate.AddDays(1) &&
                      io.OperationTypeId == IncomeOperation.IOType.New
                group income by income.DeskGroupId
                into g
                select new
                {
                    g.Key,
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var expenditureList = (from expenditure in _db.Expenditures
                    .Include(e => e.DeskGroup)
                    .Include(e => e.IncomingExpenditure).ThenInclude(ie => ie.FinancialAccountOperation)
                from eo in _db.ExpenditureOperations.Where(eo => expenditure.ExpenditureId == eo.ExpenditureId)
                    .OrderByDescending(eo => eo.OperationDateTime).Take(1).DefaultIfEmpty()
                where ((expenditure.PaymentTypeId == PaymentTypes.Cash && eo.OperationDateTime >= queryFromDate &&
                        eo.OperationDateTime < queryToDate.AddDays(1)) ||
                       (expenditure.PaymentTypeId == PaymentTypes.Cashless &&
                        expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime >=
                        queryFromDate &&
                        expenditure.IncomingExpenditure.FinancialAccountOperation.OperationDateTime <
                        queryToDate.AddDays(1))) &&
                      eo.OperationTypeId == ExpenditureOperation.EOType.New
                group expenditure by expenditure.DeskGroupId
                into g
                select new
                {
                    g.Key,
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var salesList = (from dg in _db.DeskGroups
                     join d in _db.Desks on dg.DeskGroupId equals d.GroupId
                     join s in _db.VBookingManagementSales on d.DeskId equals s.DeskId
                     where s.ExecutionDateTime >= queryFromDate && s.ExecutionDateTime < queryToDate.AddDays(1)
                     select new
                     {
                         dg.DeskGroupId,
                         s.TicketID,
                         s.OperationTypeID,
                         Amount = (s.OperationTypeID == 2 || s.OperationTypeID == 6) ?
                            (-s.CashAmount - s.PKAmount - s.BNAmount) :
                            ((s.OperationTypeID == 3 || s.OperationTypeID == 7) ?
                            (s.ChildCashAmount + s.ChildPKAmount + s.ChildBNAmount + s.ChildCashPenalty + s.ChildBNPenalty + s.ChildPKPenalty) :
                            (s.CashAmount + s.PKAmount + s.BNAmount))
                     }).GroupBy(ig => new { ig.TicketID, ig.OperationTypeID }).Select(g => new
                     {
                         opTypeId = g.Key.OperationTypeID,
                         Amount = g.Min(s => s.Amount),
                         DeskGroupId = g.Min(s => s.DeskGroupId)
                     }).ToList().GroupBy(ig => ig.DeskGroupId)
                        .Select(g => new
                        {
                            g.Key,
                            Amount = g.Sum(ig => ig.Amount)
                        }).ToList();

            var model = new FinalSummaryViewModel
            {
                FromDate = queryFromDate.ToString("d"),
                ToDate = queryToDate.ToString("d"),
                Items = (from dg in _db.DeskGroups
                         from k in KRSList.Where(k => k.Key == dg.DeskGroupId).DefaultIfEmpty()
                         from ct in CorpTicketList.Where(c => c.Key == dg.DeskGroupId).DefaultIfEmpty()
                         from cl in CorpLuggageList.Where(c => c.Key == dg.DeskGroupId).DefaultIfEmpty()
                         from ol in otherIncomeList.Where(c => c.Key == dg.DeskGroupId).DefaultIfEmpty()
                         from e in expenditureList.Where(e => e.Key == dg.DeskGroupId).DefaultIfEmpty()
                         from s in salesList.Where(s => s.Key == dg.DeskGroupId).DefaultIfEmpty()
                        from cif in customIncomeList.Where(cif => cif.Key == dg.DeskGroupId).DefaultIfEmpty()
                         select new FinalSummaryViewItem
                         {
                             Name = dg.Name,
                             DeskGroupId = dg.DeskGroupId,
                             IncomeAmount = (k == null ? 0 : k.Amount) + (ct == null ? 0 : ct.Amount) + (cl == null ? 0 : cl.Amount) + (cif == null ? 0 : cif.Amount) + (ol == null ? 0 : ol.Amount),
                             ExpenditureAmount = (e == null ? 0 : e.Amount),
                             SalesAmount = (s == null ? 0 : s.Amount)
                         }).ToList()
            };

            model.Items.Add(model.Items.GroupBy(g => 1).Select(g => new FinalSummaryViewItem
            {
                Name = "Итого",
                IncomeAmount = g.Sum(ig => ig.IncomeAmount),
                ExpenditureAmount = g.Sum(ig => ig.ExpenditureAmount),
                SalesAmount = g.Sum(ig => ig.SalesAmount)
            }).First());

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult CreateIncome()
        {
            var model = new CreateIncomeModel()
            {
                DeskGroups = from d in _db.DeskGroups.Where(g => g.IsActive).OrderBy(g => g.Name)
                             select new SelectListItem
                             {
                                 Value = d.DeskGroupId.ToString(),
                                 Text = d.Name
                             }
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateIncome(CreateIncomeModel model)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            //var officeRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Офис"));
            //var office = _db.Users.FirstOrDefault(u => u.Roles.Any(r => r.RoleId == officeRole.Id));
            //if (office == null) return RedirectToAction("IncomeSummary");

            //var otherSum = decimal.Parse(_db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value
            //    .Replace(".", ",").Replace(" ", string.Empty));
            //var remainder = model.Amount;
            //if (otherSum > 0)
            //{
            //    _db.SettingsValues.FirstOrDefault(sv => sv.Key == "OtherSum").Value = otherSum >= model.Amount
            //        ? (otherSum - model.Amount).ToString("#,0", nfi)
            //        : (0).ToString("#,0", nfi);
            //}

            var income = new Income
            {
                Name = model.Name,
                Amount = model.Amount,
                DeskGroupId = model.SelectedDeskGroupId
            };

            var operation = new IncomeOperation
            {
                Income = income,
                OperationDateTime = DateTime.Now,
                OperationTypeId = IncomeOperation.IOType.New
            };

            //var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            using (var transaction = await _db.Database.BeginTransactionAsync())
            {
                _db.IncomeOperations.Add(operation);
                //_db.SetUserContext(user.Id);
                await _db.SaveChangesAsync();

                transaction.Commit();
            }

            return RedirectToAction("IncomeSummary");
        }
    }
}