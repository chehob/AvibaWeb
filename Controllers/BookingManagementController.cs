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
using Microsoft.AspNetCore.Razor.Language.Intermediate;

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
        public async Task<IActionResult> Sales(DateTime? fromDate, DateTime? toDate, string[] deskFilter, string[] sessionFilter, string[] airlineFilter, string[] originCity, string[] destinationCity)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate;
            var railTicketTypes = new int[] { 3, 5 };

            var ticketInfo = (from s in _db.VBookingManagementSales
                              where s.ExecutionDateTime >= queryFromDate && s.ExecutionDateTime < queryToDate.AddDays(1)
                                && deskFilter.Contains(s.DeskId) && sessionFilter.Contains(s.Session)
                                && (airlineFilter.Length == 0 || airlineFilter.Contains(s.AirlineCode) || airlineFilter.Contains(s.AirlineCode))
                                && (originCity.Length == 0 || originCity.Contains(s.Origin) || originCity.Contains(s.OriginEn))
                                && (destinationCity.Length == 0 || destinationCity.Contains(s.Destination) || destinationCity.Contains(s.DestinationEn))
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

        [HttpPost]
        public async Task<IActionResult> KRS(DateTime? fromDate, DateTime? toDate, string[] deskFilter, string[] sessionFilter, string[] airlineFilter)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate;
            var railTicketTypes = new int[] { 3, 5 };

            var KRSList = (from info in _db.VServiceReceiptIncomeInfo
                where info.DateTime >= queryFromDate && info.DateTime < queryToDate.AddDays(1) &&
                      deskFilter.Contains(info.DeskIssuedId) //&& sessionFilter.Contains(ti.InfoSession)
                           select new
                           {
                               info.TicketOpTypeId,
                               info.SegCount,
                               IsSite = info.Serie == "ТКП" ? 1 : 0,
                               info.Amount
                           }).GroupBy(g => new { g.TicketOpTypeId, g.IsSite })
                .Select(g => new
                {
                    OpTypeId = g.Key.TicketOpTypeId,
                    g.Key.IsSite,
                    ServiceCount = g.Count(),
                    SegCount = g.Sum(ig => ig.SegCount),
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var KRSFilterList = (from info in _db.VServiceReceiptIncomeInfo
                    where info.DateTime >= queryFromDate && info.DateTime < queryToDate.AddDays(1) &&
                          deskFilter.Contains(info.DeskIssuedId) && info.IsFiltered == 1 && info.Serie == "ТКП"
                    select new
                    {
                        info.TicketOpTypeId,
                        info.SegCount,
                        info.Amount
                    }).GroupBy(g => new { g.TicketOpTypeId })
                .Select(g => new
                {
                    OpTypeId = g.Key.TicketOpTypeId,
                    ServiceCount = g.Count(),
                    SegCount = g.Sum(ig => ig.SegCount),
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var CorpTicketList = (from ti in _db.VReceiptTicketInfo
                    join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                    join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                    where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) &&
                          deskFilter.Contains(ti.DeskId) && sessionFilter.Contains(ti.InfoSession) &&
                          cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                          cr.TypeId == CorporatorReceipt.CRType.CorpClient &&
                          cri.TypeId == CorporatorReceiptItem.CRIType.Ticket
                    select new
                    {
                        ti.TicketOpTypeId,
                        ti.SegCount,
                        Amount = cri.IsPercent ? cri.Amount * cri.FeeRate / 100 :
                            cri.PerSegment ? cri.FeeRate * ti.SegCount : cri.FeeRate
                    }).GroupBy(g => g.TicketOpTypeId)
                .Select(g => new
                {
                    OpTypeId = g.Key,
                    ServiceCount = g.Count(),
                    SegCount = g.Sum(ig => ig.SegCount),
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var CorpLuggageList = (from ti in _db.VReceiptLuggageInfo
                    join cri in _db.CorporatorReceiptItems on ti.TicketOperationId equals cri.TicketOperationId
                    join cr in _db.CorporatorReceipts on cri.CorporatorReceiptId equals cr.CorporatorReceiptId
                    where cr.PaidDateTime >= queryFromDate && cr.PaidDateTime < queryToDate.AddDays(1) &&
                          deskFilter.Contains(ti.DeskId) && sessionFilter.Contains(ti.InfoSession) &&
                          cr.StatusId == CorporatorReceipt.CRPaymentStatus.Paid &&
                          cr.TypeId == CorporatorReceipt.CRType.CorpClient &&
                          cri.TypeId == CorporatorReceiptItem.CRIType.Luggage
                    select new
                    {
                        ti.TicketOpTypeId,
                        Amount = cri.IsPercent ? cri.Amount * cri.FeeRate / 100 : cri.FeeRate
                    }).GroupBy(g => g.TicketOpTypeId)
                .Select(g => new
                {
                    OpTypeId = g.Key,
                    ServiceCount = g.Count(),
                    SegCount = g.Count(),
                    Amount = g.Sum(ig => ig.Amount)
                }).ToList();

            var BNModel = (from op in new[] {1, 2, 3, 4, 5, 6, 7, 8}
                from ct in CorpTicketList.Where(ct => ct.OpTypeId == op).DefaultIfEmpty()
                from cl in CorpLuggageList.Where(cl => cl.OpTypeId == op).DefaultIfEmpty()
                select new
                {
                    OpTypeId = op,
                    ServiceCount = (ct?.ServiceCount ?? 0) + (cl?.ServiceCount ?? 0),
                    SegCount = (ct?.SegCount ?? 0) + (cl?.SegCount ?? 0),
                    Amount = (ct?.Amount ?? 0) + (cl?.Amount ?? 0),
                }).ToList();

            var model = new KRSViewModel
            {
                CashSale = KRSList.Where(ti => ti.OpTypeId == 1 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                CashExchange = KRSList.Where(ti => ti.OpTypeId == 3 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                CashRefund = KRSList.Where(ti => ti.OpTypeId == 2 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                CashForcedRefund = KRSList.Where(ti => ti.OpTypeId == 6 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                CashService = KRSList.Where(ti => ti.OpTypeId == 10 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                CashCancel = KRSList.Where(ti => ti.OpTypeId == 5 && ti.IsSite == 0).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKSale = KRSList.Where(ti => ti.OpTypeId == 1 && ti.IsSite == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKExchange = KRSList.Where(ti => ti.OpTypeId == 3 && ti.IsSite == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKRefund = KRSList.Where(ti => ti.OpTypeId == 2 && ti.IsSite == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKCancel = KRSList.Where(ti => ti.OpTypeId == 5 && ti.IsSite == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKFilterSale = KRSFilterList.Where(ti => ti.OpTypeId == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKFilterExchange = KRSFilterList.Where(ti => ti.OpTypeId == 3).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKFilterRefund = KRSFilterList.Where(ti => ti.OpTypeId == 2).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                PKFilterCancel = KRSFilterList.Where(ti => ti.OpTypeId == 5).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                BNSale = BNModel.Where(ti => ti.OpTypeId == 1).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                BNExchange = BNModel.Where(ti => ti.OpTypeId == 3).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                BNRefund = BNModel.Where(ti => ti.OpTypeId == 2).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
                BNForcedRefund = BNModel.Where(ti => ti.OpTypeId == 6).DefaultIfEmpty().Select(ti =>
                    new KRSViewItem
                    {
                        KRSCount = ti?.ServiceCount ?? 0,
                        Amount = ti?.Amount ?? 0,
                        SegCount = ti?.SegCount ?? 0
                    }).First(),
            };

            model.CashTotal = new KRSViewItem
            {
                Amount = model.CashSale.Amount + model.CashExchange.Amount + model.CashRefund.Amount + model.CashService.Amount + model.CashCancel.Amount,
                SegCount = model.CashSale.SegCount + model.CashExchange.SegCount + model.CashRefund.SegCount + model.CashService.SegCount - model.CashCancel.SegCount,
                KRSCount = model.CashSale.KRSCount + model.CashExchange.KRSCount + model.CashRefund.KRSCount + model.CashService.KRSCount - model.CashCancel.KRSCount
            };

            model.PKTotal = new KRSViewItem
            {
                Amount = model.PKSale.Amount + model.PKExchange.Amount + model.PKRefund.Amount,
                KRSCount = model.PKSale.KRSCount + model.PKExchange.KRSCount + model.PKRefund.KRSCount
            };

            model.PKFilterTotal = new KRSViewItem
            {
                Amount = model.PKFilterSale.Amount + model.PKFilterExchange.Amount + model.PKFilterRefund.Amount,
                KRSCount = model.PKFilterSale.KRSCount + model.PKFilterExchange.KRSCount + model.PKFilterRefund.KRSCount
            };

            model.BNTotal = new KRSViewItem
            {
                Amount = model.BNSale.Amount + model.BNExchange.Amount + model.BNRefund.Amount,
                SegCount = model.BNSale.SegCount + model.BNExchange.SegCount + model.BNRefund.SegCount,
                KRSCount = model.BNSale.KRSCount + model.BNExchange.KRSCount + model.BNRefund.KRSCount
            };

            model.FinalTotalStr = (model.CashTotal.Amount + model.PKTotal.Amount + model.BNTotal.Amount).ToString("#,0.00", nfi);
            model.FinalTotalCommStr = (model.CashTotal.Amount + model.PKTotal.AmountComm + model.BNTotal.Amount).ToString("#,0.00", nfi);

            return Json(new { message = await _viewRenderService.RenderToStringAsync("BookingManagement/KRS", model) });
        }

        [HttpPost]
        public async Task<IActionResult> Luggage(DateTime? fromDate, DateTime? toDate, string[] deskFilter, string[] sessionFilter, string[] airlineFilter)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate;
            var railTicketTypes = new int[] { 3, 5 };

            var model = new LuggageViewModel
            {
                Items = (from info in _db.VBookingManagementLuggage
                         where info.DateTime >= queryFromDate && info.DateTime < queryToDate.AddDays(1) &&
                              deskFilter.Contains(info.DeskID) && sessionFilter.Contains(info.Session)
                        select new
                        {
                            info.Airline,
                            Weight = info.LuggageAmount,
                            Amount = info.LuggageAmount * info.LuggageRate
                        }).GroupBy(g => g.Airline)
                    .Select(g => new LuggageViewItem
                    {
                        Airline = g.Key,
                        DocCount = g.Count(),
                        Amount = g.Sum(ig => ig.Amount),
                        Weight = g.Sum(ig => ig.Weight)
                    }).ToList()
            };

            model.Items.Add(new LuggageViewItem
            {
                Airline = "Итого",
                DocCount = model.Items.Sum(i => i.DocCount),
                Weight = model.Items.Sum(i => i.Weight),
                Amount = model.Items.Sum(i => i.Amount)
            });

            return Json(new { message = await _viewRenderService.RenderToStringAsync("BookingManagement/Luggage", model) });
        }

        [HttpPost]
        public async Task<IActionResult> Operations(DateTime? fromDate, DateTime? toDate, string[] deskFilter, string[] sessionFilter, string[] airlineFilter)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var queryToDate = toDate ?? DateTime.Now.Date;
            var queryFromDate = fromDate ?? queryToDate;

            var model = new OperationsViewModel
            {
                Items = (from info in _db.VBookingManagementOperations
                        where info.ExecutionDateTime >= queryFromDate && info.ExecutionDateTime < queryToDate.AddDays(1) &&
                              deskFilter.Contains(info.DeskID) && sessionFilter.Contains(info.Session)
                        orderby info.ExecutionDateTime 
                        select new OperationsViewItem
                        {
                            TicketNumber = info.TicketID,
                            Airline = info.Airline,
                            Route = info.Flight,
                            DepartureDateTime = info.FlightDate.ToString("g"),
                            OperationType = info.OperationType,
                            TicketCost = (info.Fare + info.TaxAmount).ToString("#,0.00", nfi),
                            ServiceTax = info.KRSTax,
                            Penalty = info.Penalty.ToString("#,0.00", nfi),
                            PassengerName = info.FullName,
                            Phone = info.Phone,
                            Email = info.Email,
                            Desk = info.DeskID,
                            OperationDateTime = info.ExecutionDateTime.ToString("g"),
                            BookDesk = info.BookDeskID,
                            BookDateTime = info.BookDateTime == null ? "" : info.BookDateTime.Value.ToString("g"),
                            PaymentType = info.PaymentType
                        }).ToList()
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("BookingManagement/Operations", model) });
        }

        [HttpGet]
        public IActionResult SearchOperations()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> SearchOperations(string key)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            string lowerKey = null;
            if (key != null)
            {
                lowerKey = key.ToLower();
            }

            var model = new OperationsViewModel
            {
                Items = (from info in _db.VBookingManagementOperations
                         where info.TicketID.ToLower().Contains(lowerKey) ||
                               info.FullName.ToLower().Contains(lowerKey) ||
                               info.PNRID.ToLower().Contains(lowerKey)
                         orderby info.ExecutionDateTime
                         select new OperationsViewItem
                         {
                             TicketNumber = info.TicketID,
                             Airline = info.Airline,
                             Route = info.Flight,
                             DepartureDateTime = info.FlightDate.ToString("g"),
                             OperationType = info.OperationType,
                             TicketCost = (info.Fare + info.TaxAmount).ToString("#,0.00", nfi),
                             ServiceTax = info.KRSTax,
                             Penalty = info.Penalty.ToString("#,0.00", nfi),
                             PassengerName = info.FullName,
                             Phone = info.Phone,
                             Email = info.Email,
                             Desk = info.DeskID,
                             OperationDateTime = info.ExecutionDateTime.ToString("g"),
                             BookDesk = info.BookDeskID,
                             BookDateTime = info.BookDateTime == null ? "" : info.BookDateTime.Value.ToString("g"),
                             PaymentType = info.PaymentType,
                             BirthDate = info.BirthDate == null ? "" : info.BirthDate.Value.ToString("d"),
                             Passport = info.Passport
                         }).ToList()
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("BookingManagement/Operations", model) });
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