﻿using System;
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
            var avibaDeskFilter = new[] { "ГРБЕ05", "ГРБЕ06", "ГРБЕ07", "ГРБЕ21", "ГРБЕ22", "ГРБЕ35", "ГРБЕ33" };
            var aviaTourDeskFilter = new[] { "ГРБЕ11", "ГРБЕ12", "ГРБЕ13", "ГРБЕ14", "ГРБЕ15", "ГРБЕ16", "ГРБЕ17", "ГРБЕ18", "ГРБЕ41" };
            var model = new OnlineInfoModel
            {
                AvibaBalanceInfo = from v in _db.VDeskBalances
                    where avibaDeskFilter.Contains(v.DeskId) && v.Balance != 0
                    orderby v.DeskName
                    select new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = v.DeskName,
                        Balance = v.Balance
                    },
                AviaTourBalanceInfo = from v in _db.VDeskBalances
                    where aviaTourDeskFilter.Contains(v.DeskId) && v.Balance != 0
                    orderby v.DeskName
                    select new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = v.DeskName,
                        Balance = v.Balance
                    },
                CollectorsBalanceInfo = _db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == collectorRole.Id && u.Balance != 0) &&
                                u.Roles.All(r => r.RoleId != officeRole.Id) &&
                                (u.Name != "Инкассатор" || (u.Name == "Инкассатор" && isUserAdmin)) )
                    .Select(u => new OnlineInfoModel.BalanceInfoElement
                    {
                        Name = u.Name,
                        Balance = u.Balance
                    }),
                OfficeBalance = _db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).Sum(u => u.Balance),
                TransitBalance = _db.TransitAccounts.FirstOrDefault().Balance
            };

            model.OfficeBillInfo = new OfficeBillInfoViewModel
            {
                _5kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "5kBillSum").Value,
                _2kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum").Value                
            };

            model.OfficeBillInfo.RemainderSum = (model.OfficeBalance - 
                decimal.Parse(model.OfficeBillInfo._5kBillSum.Replace(".", ",").Replace(" ", string.Empty)) -
                decimal.Parse(model.OfficeBillInfo._2kBillSum.Replace(".", ",").Replace(" ", string.Empty)))
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
                            let UploadDate = og.OrderByDescending(o => o.InsertDateTime).FirstOrDefault().InsertDateTime
                            select new OrganizationAccountBalance
                            {
                                Id = fa.FinancialAccountId,
                                LatestUpload = UploadDate.Date.Equals(DateTime.Now.Date) ? UploadDate.ToString("t") : UploadDate.ToString("dd.MM hh:mm"),
                                Account = fa.Description,
                                BankName = fa.BankName,
                                Balance = og.Sum(ao => ao.Amount).ToString("#,0.00", nfi),
                                Debit = og.Where(ao => ao.OperationDateTime >= reportDate)
                                    .Sum(d => d.Amount >= 0 ? d.Amount : 0).ToString("#,0.00", nfi),
                                Credit = og.Where(ao => ao.OperationDateTime >= reportDate)
                                    .Sum(cr => cr.Amount < 0 ? -cr.Amount : 0).ToString("#,0.00", nfi)
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
                _2kBillSum = _db.SettingsValues.FirstOrDefault(sv => sv.Key == "2kBillSum").Value
            };

            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));

            model.RemainderSum = (_db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).Sum(u => u.Balance) -
                decimal.Parse(model._5kBillSum.Replace(".", ",").Replace(" ", string.Empty)) -
                decimal.Parse(model._2kBillSum.Replace(".", ",").Replace(" ", string.Empty)))
                .ToString("#,0", nfi);

            return PartialView(model);
        }        

        [HttpGet]
        public ActionResult AccountOperations(int accountId)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from fa in _db.FinancialAccounts
                where fa.FinancialAccountId == accountId
                select new AccountOperationsViewModel
                {
                    OrgName = fa.Organization.Description,
                    BankName = fa.BankName,
                    Operations =
                        (from fao in _db.FinancialAccountOperations
                            where fao.FinancialAccountId == accountId
                            orderby fao.OperationDateTime descending
                            select new AccountOperationData
                            {
                                OperationDateTime = fao.OperationDateTime.ToString("d"),
                                OrderNumber = fao.OrderNumber,
                                CounterpartyName =
                                    fao.TransferAccount != null
                                        ? fao.TransferAccount.Organization.Description
                                        : (fao.Counterparty != null
                                            ? fao.Counterparty.Name
                                            : (fao.PayeeUser != null ? fao.PayeeUser.Name : "")),
                                Amount = fao.Amount,
                                AmountStr = fao.Amount.ToString("#,0.00", nfi),
                                Description = fao.Description
                            }).ToList()
                }).FirstOrDefault();
            return PartialView(model);
        }
    }
}