using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.BookingManagement;
using System.Globalization;
using AvibaWeb.Infrastructure;

namespace AvibaWeb.Controllers
{
    public class BookingManagementController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly IViewRenderService _viewRenderService;

        public BookingManagementController(AppIdentityDbContext db, IViewRenderService viewRenderService)
        {
            _db = db;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Sales(DateTime? fromDate, DateTime? toDate, string[] deskFilter, string[] sessionFilter)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate;
            var railTicketTypes = new int[] { 3, 5 };

            var ticketInfo = (from s in _db.VBookingManagementSales
                              where s.ExecutionDateTime >= queryFromDate && s.ExecutionDateTime < queryToDate.AddDays(1)
                                && deskFilter.Contains(s.DeskId) && sessionFilter.Contains(s.Session)
                              select new
                              {
                                  s.TicketID,
                                  TypeID = railTicketTypes.Contains(s.TypeId) ? 1 : 0,
                                  s.OperationTypeID,
                                  s.CashAmount,
                                  s.ChildCashAmount,
                                  s.PKAmount,
                                  s.ChildPKAmount,
                                  s.BNAmount,
                                  s.ChildBNAmount,
                                  s.PenaltyCount,
                                  s.CashPenalty,
                                  s.PKPenalty,
                                  s.BNPenalty,
                                  s.ChildCashPenalty,
                                  s.ChildPKPenalty,
                                  s.ChildBNPenalty
                              }).GroupBy(ig => new { ig.TicketID, ig.OperationTypeID })
                              .Select(g => new
                              {
                                  typeId = g.Min(s => s.TypeID),
                                  opTypeId = g.Key.OperationTypeID,
                                  SegCount = g.Count(),
                                  CashAmount = g.Min(s => s.CashAmount),
                                  ChildCashAmount = g.Min(s => s.ChildCashAmount),
                                  PKAmount = g.Min(s => s.PKAmount),
                                  ChildPKAmount = g.Min(s => s.ChildPKAmount),
                                  BNAmount = g.Min(s => s.BNAmount),
                                  ChildBNAmount = g.Min(s => s.ChildBNAmount),
                                  CashPenalty = g.Min(s => s.CashPenalty),
                                  PKPenalty = g.Min(s => s.PKPenalty),
                                  BNPenalty = g.Min(s => s.BNPenalty),
                                  ChildCashPenalty = g.Min(s => s.ChildCashPenalty),
                                  ChildPKPenalty = g.Min(s => s.ChildPKPenalty),
                                  ChildBNPenalty = g.Min(s => s.ChildBNPenalty),
                                  PenaltyCount = g.Min(s => s.PenaltyCount)
                              }).ToList()
                                .GroupBy(ig => new { ig.typeId, ig.opTypeId })
                                .Select(g => new
                                {
                                    g.Key.typeId,
                                    g.Key.opTypeId,
                                    SegCount = g.Sum(s => s.SegCount),
                                    CashAmount = g.Sum(s => s.CashAmount),
                                    ChildCashAmount = g.Sum(s => s.ChildCashAmount),
                                    PKAmount = g.Sum(s => s.PKAmount),
                                    ChildPKAmount = g.Sum(s => s.ChildPKAmount),
                                    BNAmount = g.Sum(s => s.BNAmount),
                                    ChildBNAmount = g.Sum(s => s.ChildBNAmount),
                                    CashPenalty = g.Sum(s => s.CashPenalty),
                                    PKPenalty = g.Sum(s => s.PKPenalty),
                                    BNPenalty = g.Sum(s => s.BNPenalty),
                                    ChildCashPenalty = g.Sum(s => s.ChildCashPenalty),
                                    ChildPKPenalty = g.Sum(s => s.ChildPKPenalty),
                                    ChildBNPenalty = g.Sum(s => s.ChildBNPenalty),
                                    PenaltyCount = g.Sum(s => s.PenaltyCount)
                                }).ToList();

            var model = new SalesViewModel
            {
                AirSale = ticketInfo.Where(ti => ti.opTypeId == 1 && ti.typeId == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirPenalty = ticketInfo.Where(ti => ti.typeId == 0).GroupBy(g => g.typeId).DefaultIfEmpty().Select(g =>
                new SalesViewItem
                {
                    AmountCash = g?.Sum(ti => (ti?.CashPenalty ?? 0) + (ti?.ChildCashPenalty ?? 0)) ?? 0,
                    AmountPK = g?.Sum(ti => (ti?.PKPenalty ?? 0) + (ti?.ChildPKPenalty ?? 0)) ?? 0,
                    AmountBN = g?.Sum(ti => (ti?.BNPenalty ?? 0) + (ti?.ChildBNPenalty ?? 0)) ?? 0,
                    SegCount = g?.Sum(ti => ti?.PenaltyCount ?? 0) ?? 0
                }).First(),
                AirExchange = ticketInfo.Where(ti => ti.opTypeId == 3 && ti.typeId == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.ChildCashAmount ?? 0,
                    AmountPK = ti?.ChildPKAmount ?? 0,
                    AmountBN = ti?.ChildBNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirRefund = ticketInfo.Where(ti => ti.opTypeId == 2 && ti.typeId == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = (ti?.CashAmount ?? 0) + (ti?.CashPenalty ?? 0),
                    AmountPK = (ti?.PKAmount ?? 0) + (ti?.PKPenalty ?? 0),
                    AmountBN = (ti?.BNAmount ?? 0) + (ti?.BNPenalty ?? 0),
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirForcedExchange = ticketInfo.Where(ti => ti.opTypeId == 7 && ti.typeId == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.ChildCashAmount ?? 0,
                    AmountPK = ti?.ChildPKAmount ?? 0,
                    AmountBN = ti?.ChildBNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirForcedRefund = ticketInfo.Where(ti => ti.opTypeId == 6 && ti.typeId == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                RailSale = ticketInfo.Where(ti => ti.opTypeId == 1 && ti.typeId == 1).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                RailRefund = ticketInfo.Where(ti => ti.opTypeId == 2 && ti.typeId == 1).DefaultIfEmpty().Select(ti =>
                   new SalesViewItem
                   {
                       AmountCash = ti?.CashAmount ?? 0,
                       AmountPK = ti?.PKAmount ?? 0,
                       AmountBN = ti?.BNAmount ?? 0,
                       SegCount = ti?.SegCount ?? 0
                   }).First()
            };

            model.AirTotal = new SalesViewItem
            {
                AmountCash = model.AirSale.AmountCash + model.AirPenalty.AmountCash + model.AirExchange.AmountCash -
                    model.AirRefund.AmountCash + model.AirForcedExchange.AmountCash - model.AirForcedRefund.AmountCash,
                AmountPK = model.AirSale.AmountPK + model.AirPenalty.AmountPK + model.AirExchange.AmountPK -
                    model.AirRefund.AmountPK + model.AirForcedExchange.AmountPK - model.AirForcedRefund.AmountPK,
                AmountBN = model.AirSale.AmountBN + model.AirPenalty.AmountBN + model.AirExchange.AmountBN -
                    model.AirRefund.AmountBN + model.AirForcedExchange.AmountBN - model.AirForcedRefund.AmountBN,
                SegCount = model.AirSale.SegCount + model.AirExchange.SegCount + model.AirRefund.SegCount,
            };

            model.FinalTotal = new SalesViewItem
            {
                AmountCash = model.AirTotal.AmountCash + model.RailSale.AmountCash - model.RailRefund.AmountCash,
                AmountPK = model.AirTotal.AmountPK + model.RailSale.AmountPK - model.RailRefund.AmountPK,
                AmountBN = model.AirTotal.AmountBN + model.RailSale.AmountBN - model.RailRefund.AmountBN,
                SegCount = model.AirTotal.SegCount + model.RailSale.SegCount + model.RailRefund.SegCount
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("BookingManagement/Sales", model) });
        }

        public IActionResult Filter()
        {
            var model = new FilterViewModel
            {
            };
            return PartialView(model);
        }

        public JsonResult GetDeskFilter(string query)
        {
            var deskGroups = _db.BMDeskGroups.Include(bm => bm.Desks).ToList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                deskGroups = deskGroups.Where(q => q.Name.Contains(query)).ToList();
            }

            var records = deskGroups.Where(m => m.Name == "Все")
                .Select(m => new DeskFilterItem
                {
                    id = m.BMDeskGroupId.ToString(),
                    text = m.Name,
                    icon = "false",
                    state = new DeskFilterItemState
                    {
                        opened = true,
                        selected = true
                    },
                    children = deskGroups.Where(dg => dg.Name != "Все").OrderBy(dg => dg.Name)
                        .Select(dg => new DeskFilterItem
                        {
                            id = dg.BMDeskGroupId.ToString(),
                            text = dg.Name,
                            icon = "false",
                            state = new DeskFilterItemState
                            {
                                selected = true
                            },
                            children = GetGroupDesks(dg.Desks)
                        }).ToList()
                }).ToList();

            return this.Json(records);
        }

        private List<DeskFilterItem> GetGroupDesks(ICollection<Desk> desks)
        {
            return desks.OrderBy(d => d.DeskId)
                .Select(d => new DeskFilterItem
                {
                    id = d.DeskId,
                    text = d.Description + $" ({d.DeskId})" ?? d.DeskId,
                    icon = "false",
                    state = new DeskFilterItemState
                    {
                        selected = true
                    }
                }).ToList();
        }

        public JsonResult GetSessionFilter(string query)
        {
            var sessions = _db.VSessionTypes.ToList();

            if (!string.IsNullOrWhiteSpace(query))
            {
                sessions = sessions.Where(q => q.Name.Contains(query)).ToList();
            }

            var records = new List<DeskFilterItem> {
                new DeskFilterItem
                {
                    id = "0",
                    text = "Все",
                    icon = "false",
                    state = new DeskFilterItemState
                    {
                        opened = true,
                        selected = true
                    },
                    children = sessions.OrderBy(s => s.Name)
                        .Select(s => new DeskFilterItem
                        {
                            id = s.SessionId.ToString(),
                            text = s.Name,
                            icon = "false",
                            state = new DeskFilterItemState
                            {
                                selected = true
                            }
                        }).ToList()
                }
            };

            return this.Json(records);
        }

        public JsonResult SearchCity(string query)
        {
            SelectResult result = new SelectResult();
            if (!string.IsNullOrEmpty(query))
            {
                result = new SelectResult
                {
                    results = (from c in _db.VCities
                               where c.Name.ToLower().Contains(query.ToLower())
                               select new SelectResultItem
                               {
                                   id = c.Name,
                                   text = c.Name
                               }).Take(10).ToList()
                };
            }

            return this.Json(result);
        }

        public JsonResult SearchAirline(string query)
        {
            SelectResult result = new SelectResult();
            if (!string.IsNullOrEmpty(query))
            {
                result = new SelectResult
                {
                    results = (from a in _db.VAirlines
                               where a.FullName.ToLower().Contains(query.ToLower())
                               select new SelectResultItem
                               {
                                   id = a.Code,
                                   text = a.FullName + " - " + a.Code
                               }).Take(10).ToList()
                };
            }

            return this.Json(result);
        }
    }
}