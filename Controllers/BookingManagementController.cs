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

            var ticketInfo = (from s in _db.VBookingManagementSales
                              where s.ExecutionDateTime >= queryFromDate && s.ExecutionDateTime < queryToDate.AddDays(1)
                                && deskFilter.Contains(s.DeskID) && sessionFilter.Contains(s.Session)
                              group s by new { TypeID = Array.IndexOf(new int[] { 3, 5 }, s.TypeID) != -1 ? 1 : 0, s.OperationTypeID } into g
                              select new
                              {
                                  g.Key.TypeID,
                                  g.Key.OperationTypeID,
                                  typeId = g.FirstOrDefault().TypeID,
                                  opTypeId = g.FirstOrDefault().OperationTypeID,
                                  SegCount = g.Count(),
                                  CashAmount = g.Sum(s => s.CashAmount / s.SegCount),
                                  ChildCashAmount = g.Sum(s => s.ChildCashAmount),
                                  PKAmount = g.Sum(s => s.PKAmount / s.SegCount),
                                  ChildPKAmount = g.Sum(s => s.ChildPKAmount),
                                  BNAmount = g.Sum(s => s.BNAmount / s.SegCount),
                                  ChildBNAmount = g.Sum(s => s.ChildBNAmount)
                              }).ToList();

            var model = new SalesViewModel
            {
                AirSale = ticketInfo.Where(ti => ti.OperationTypeID == 1 && ti.TypeID == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirPenalty = new SalesViewItem
                {
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                AirExchange = new SalesViewItem
                {
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                AirRefund = ticketInfo.Where(ti => ti.OperationTypeID == 2 && ti.TypeID == 0).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                AirForcedExchange = new SalesViewItem
                {
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                AirForcedRefund = new SalesViewItem
                {
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                RailSale = ticketInfo.Where(ti => ti.OperationTypeID == 1 && ti.TypeID == 1).DefaultIfEmpty().Select(ti =>
                new SalesViewItem
                {
                    AmountCash = ti?.CashAmount ?? 0,
                    AmountPK = ti?.PKAmount ?? 0,
                    AmountBN = ti?.BNAmount ?? 0,
                    SegCount = ti?.SegCount ?? 0
                }).First(),
                RailRefund = ticketInfo.Where(ti => ti.OperationTypeID == 2 && ti.TypeID == 1).DefaultIfEmpty().Select(ti =>
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