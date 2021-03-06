﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.ManagementAccountingViewModels;
using Microsoft.AspNetCore.Internal;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using MoreLinq;

namespace AvibaWeb.Controllers
{
    public class ManagementAccountingController : Controller
    {
        private readonly AppIdentityDbContext _db;

        public ManagementAccountingController(AppIdentityDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> ProviderBalance()
        {
            var model = await (from c in _db.Counterparties
                    .Include(c => c.ProviderBinding)
                where c.Type.Description == "Провайдер услуг"
                select c).ToListAsync();

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cash()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));
            var collectorRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Инкассаторы"));
            var deskFilter = new[] { "ГРБЕ53", "ГРБЕ54", "ГРБЕ55", "ГРБЕ21", "ГРБЕ22", "ГРБЕ35", "ГРБЕ33", "ГРБЕ56", "ГРБЕ12", "ГРБЕ58", "ГРБЕ57", "ГРБЕ59", "ГРБЕ16", "ГРБЕ61", "ГРБЕ60", "ГРБЕ41" };
            var model = new CashBlockViewModel
            {
                DeskBalance = await (from v in _db.VDeskBalances
                                     where deskFilter.Contains(v.DeskId) && v.Balance != 0
                                     select v)
                    .SumAsync(v => v.Balance),
                CollectorsBalance = await _db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == collectorRole.Id && u.Balance != 0) &&
                                u.Roles.All(r => r.RoleId != officeRole.Id))
                    .SumAsync(u => u.Balance),
                OfficeBalance = await _db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).SumAsync(u => u.Balance),
                TransitBalance = _db.TransitAccounts.FirstOrDefault().Balance
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Cashless()
        {
            var model = new CashlessBlockViewModel
            {
                Organizations = await (from o in _db.Organizations
                    join fa in _db.FinancialAccounts on o.OrganizationId equals fa.OrganizationId
                    group new {o, fa} by o.OrganizationId
                    into g
                    select new OrganizationCashlessInfo
                    {
                        Name = g.FirstOrDefault().o.Description,
                        Balance = g.Sum(gr => gr.fa.Balance)
                    }).ToListAsync()
            };

            var beginIndex = model.Organizations.FindIndex(o => o.Name == "ТКП");
            model.Organizations = model.Organizations.Move(beginIndex, 1, model.Organizations.Count - 1).ToList();
            beginIndex = model.Organizations.FindIndex(o => o.Name == "ИМ");
            model.Organizations = model.Organizations.Move(beginIndex, 1, model.Organizations.Count - 1).ToList();

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Corporators()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var items = await (from v in _db.CorporatorAccounts
                         join c in _db.Counterparties.Include(c => c.Type) on v.ITN equals c.ITN
                         where v.Balance != 0
                         orderby v.Balance
                         where c.Type.Description == "Корпоратор" && c.LoanGroupId == null
                         select new
                         {
                             v.Balance
                         }).ToListAsync();

            var model = new CorporatorBlockViewModel
            {
                NegBalance = items.Where(i => i.Balance < 0).Sum(i => i.Balance).ToString("#,0.00", nfi),
                PosBalance = items.Where(i => i.Balance >= 0).Sum(i => i.Balance).ToString("#,0.00", nfi),
                TotalStr = items.Sum(i => i.Balance).ToString("#,0.00", nfi)
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Expenditures()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Providers()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var alfaBankBalance =
                (from fa in _db.FinancialAccounts
                 where fa.Description == "40702810102970003030"
                 select fa.Balance).FirstOrDefault();

            var model = new ProvidersBlockViewModel
            {
                Organizations = await (from c in _db.Counterparties
                        .Include(c => c.ProviderBalance)
                    where c.Type.Description == "Провайдер услуг"
                    select new OrganizationCashlessInfo
                    {
                        Name = c.Name,
                        Balance = c.Name == "ПАО \"Авиакомпания \"Сибирь\"" ? 0 : c.ProviderBalance.Balance
                    }).ToListAsync()
            };

            var s7balance = (from pb in _db.S7ProviderBalance
                where pb.Id == 1
                select pb.Balance).FirstOrDefault();

            model.Organizations.FirstOrDefault(o => o.Name == "ПАО \"Авиакомпания \"Сибирь\"").Balance =
                alfaBankBalance + s7balance;

            var tkpOrg = model.Organizations.FirstOrDefault(o => o.Name == "АО \"Транспортная Клиринговая палата\"");
            tkpOrg.CustomBalanceStr = (2122000 + tkpOrg.Balance).ToString("#,0.00", nfi);

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Subagents()
        {
            var model = new CashlessBlockViewModel
            {
                Organizations = await (from c in _db.Counterparties
                        .Include(c => c.SubagentData)
                    where c.Type.Description == "Субагент Р" && c.SubagentData.Balance != 0
                    select new OrganizationCashlessInfo
                    {
                        Name = c.Name,
                        Balance = -c.SubagentData.Balance
                    }).ToListAsync()
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Sales()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Transit()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new CashlessBlockViewModel
            {
                Organizations = await (from lg in _db.LoanGroups
                    where lg.IsActive && lg.Description != "Дивиденты" && lg.Balance != 0
                    select new OrganizationCashlessInfo
                    {
                        Name = lg.Description,
                        Balance = lg.Balance
                    }).ToListAsync()
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> Summary()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var officeRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Офис"));
            var collectorRole = _db.Roles.FirstOrDefault(r => r.Name.Contains("Инкассаторы"));
            var deskFilter = new[] { "ГРБЕ53", "ГРБЕ54", "ГРБЕ55", "ГРБЕ21", "ГРБЕ22", "ГРБЕ35", "ГРБЕ33", "ГРБЕ56", "ГРБЕ12", "ГРБЕ58", "ГРБЕ57", "ГРБЕ59", "ГРБЕ16", "ГРБЕ61", "ГРБЕ60", "ГРБЕ41" };
            var cashData = new
            {
                DeskBalance = await (from v in _db.VDeskBalances
                        where deskFilter.Contains(v.DeskId) && v.Balance != 0
                        select v)
                    .SumAsync(v => v.Balance),
                CollectorsBalance = await _db.Users
                    .Where(u => u.Roles.Any(r => r.RoleId == collectorRole.Id && u.Balance != 0) &&
                                u.Roles.All(r => r.RoleId != officeRole.Id))
                    .SumAsync(u => u.Balance),
                OfficeBalance = await _db.Users.Where(u => u.Roles.Any(r => r.RoleId == officeRole.Id)).SumAsync(u => u.Balance),
                TransitBalance = _db.TransitAccounts.FirstOrDefault().Balance
            };

            var alfaBankBalance =
                (from fa in _db.FinancialAccounts
                    where fa.Description == "40702810102970003030"
                    select fa.Balance).FirstOrDefault();

            var s7Balance = (from pb in _db.S7ProviderBalance
                where pb.Id == 1
                select pb.Balance).FirstOrDefault();

            var providersBalance = await (from c in _db.Counterparties
                    .Include(c => c.ProviderBinding)
                where c.Type.Description == "Провайдер услуг" &&
                      c.Name != "ПАО \"Авиакомпания \"Сибирь\""
                select (c.ProviderBalance.Balance)).ToListAsync();

            providersBalance.Add(alfaBankBalance + s7Balance);

            var model = new SummaryBlockViewModel
            {
                OrganizationBalance = await (from o in _db.Organizations
                                             join fa in _db.FinancialAccounts on o.OrganizationId equals fa.OrganizationId
                                             where fa.IsActive != false && o.IsActive != false && fa.Description != "40702810102970003030"
                                             select fa).SumAsync(a => a.Balance),
                CorpNegativeBalance = -await (from v in _db.CorporatorAccounts
                                              join c in _db.Counterparties.Include(c => c.Type) on v.ITN equals c.ITN
                                              where v.Balance != 0
                                              orderby v.Balance
                                              where c.Type.Description == "Корпоратор" && c.LoanGroupId == null
                                              select v.Balance).SumAsync(v => v),
                TransitTotal = await (from lg in _db.LoanGroups
                                      where lg.IsActive && lg.Description != "Дивиденты" && lg.Balance != 0
                                      select lg.Balance).SumAsync(v => v),
                ProvidersTotal = providersBalance.Sum(v => v),
                SubagentsTotal = -await (from c in _db.Counterparties
                        .Include(c => c.SubagentData)
                                         where c.Type.Description == "Субагент Р"
                                         select c.SubagentData.Balance).SumAsync(v => v),
                DepositTotal = await (from c in _db.Counterparties
                                       .Include(c => c.ProviderBinding)
                                      where c.Type.Description == "Провайдер услуг"
                                      select c.ProviderBalance.Deposit).SumAsync(v => v) +
                               await (from c in _db.Counterparties
                                       .Include(c => c.SubagentData)
                                      where c.Type.Description == "Субагент Р"
                                      select c.SubagentData.Deposit).SumAsync(v => v),
                CashTotal = cashData.OfficeBalance + cashData.CollectorsBalance + cashData.TransitBalance +
                            cashData.DeskBalance
            };

            return PartialView(model);
        }
    }
}