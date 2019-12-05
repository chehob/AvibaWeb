using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.ManagementViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using X.PagedList;

namespace AvibaWeb.Controllers
{
    public class ManagementController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IViewRenderService _viewRenderService;
        private const int PageSize = 10;

        public ManagementController(AppIdentityDbContext db,
            IViewRenderService viewRenderService, UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
            _viewRenderService = viewRenderService;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult KRSCancelRequests()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from v in _db.VKRSCancelRequests
                         orderby v.DealDateTime descending
                         select new KRSCancelRequestViewModel
                         {
                             RequestId = v.KRSCancelRequestId,
                             Status = v.OperationTypeId,
                             Payment = v.Payment.ToString("#,0.00", nfi),
                             KRSAmount = v.KRSAmount.ToString("#,0.00", nfi),
                             PassengerName = v.PassengerName,
                             BSONumber = v.BSONumber,
                             KRSNumber = v.KRSNumber,
                             Route = v.Route,
                             DealDateTime = v.DealDateTime,
                             Description = v.Description,
                             CashierName = v.Name,
                             Desk = v.Desk,
                             ManagerName = v.ManagerName,
                             TicketStatus = v.OperationType
                         }).ToList();

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> AcceptKRSCancellation(int id)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var request = await _db.KRSCancelRequests.FirstOrDefaultAsync(r => r.KRSCancelRequestId == id);
            if (request == null) return PartialView("Error", new[] { "Запрос не найден" });

            var operation = new KRSCancelRequestOperation
            {
                Request = request,
                OperationDateTime = DateTime.Now,
                OperationTypeId = KRSCancelRequestOperation.KCROType.Accepted,
                Manager = user
            };

            _db.KRSCancelRequestOperations.Add(operation);
            await _db.SaveChangesAsync();

            var rowsAffected = _db.Database.ExecuteSqlCommand(
                "exec BookingDB.dbo.CancelKRS @KRSID = {0}, @DeskID = {1}, @Comment = {2}",
                request.KRSId, request.DeskId, request.Description);

            return RedirectToAction("KRSCancelRequests");
        }

        [HttpPost]
        public async Task<ActionResult> RejectKRSCancellation(int id)
        {
            var user = await _userManager.FindByIdAsync(_userManager.GetUserId(User));

            var request = await _db.KRSCancelRequests.FirstOrDefaultAsync(r => r.KRSCancelRequestId == id);
            if (request == null) return PartialView("Error", new[] { "Запрос не найден" });

            var operation = new KRSCancelRequestOperation
            {
                Request = request,
                OperationDateTime = DateTime.Now,
                OperationTypeId = KRSCancelRequestOperation.KCROType.Rejected,
                Manager = user
            };

            _db.KRSCancelRequestOperations.Add(operation);
            await _db.SaveChangesAsync();

            return RedirectToAction("KRSCancelRequests");
        }

        [HttpGet]
        public ActionResult TicketCancel()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> TicketList([FromBody]TicketListRequest request)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = (from v in _db.VTicketCancelList
                where v.TicketOperationDateTime >= DateTime.Parse(request.fromDate) &&
                      v.TicketOperationDateTime < DateTime.Parse(request.toDate).AddDays(1)
                orderby v.TicketOperationDateTime descending
                select new TicketCancelViewModel
                {
                    Status = v.OperationTypeId,
                    Payment = v.Payment.ToString("#,0.00", nfi),
                    PassengerName = v.PassengerName,
                    BSONumber = v.BSONumber,
                    Route = v.Route,
                    DealDateTime = v.DealDateTime,
                    TicketOperationDateTime = v.TicketOperationDateTime,
                    Description = v.Description,
                    ManagerName = v.ManagerName,
                    TicketStatus = v.OperationType,
                    TicketId = v.TicketId
                }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Management/TicketList", model) });
        }

        [HttpPost]
        public async Task<ActionResult> CancelTicket(int id, TicketCancelOperation.TCOType cancelOpType)
        {
            var operation = new TicketCancelOperation
            {
                Manager = await _userManager.FindByIdAsync(_userManager.GetUserId(User)),
                OperationDateTime = DateTime.Now,
                OperationTypeId = cancelOpType,
                TicketId = id
            };

            _db.TicketCancelOperations.Add(operation);

            var cancelQuery = "";
            switch (cancelOpType)
            {
            case TicketCancelOperation.TCOType.Accepted:
                cancelQuery = 
                    @"update BookingDB.dbo.Tickets
                    set OperationTypeID = 5
                    where ID = @TicketID

                    insert into BookingDB.dbo.TicketOperations
                    select @TicketID, 5, DeskID, getdate()
                    from BookingDB.dbo.Tickets
                    where ID = @TicketID";
                break;

            case TicketCancelOperation.TCOType.Rejected:
                cancelQuery =
                    @"update BookingDB.dbo.Tickets
                    set OperationTypeID = 1
                    where ID = @TicketID

                    delete from BookingDB.dbo.TicketOperations
                    where TicketID = @TicketID and OperationTypeID = 5";
                    break;
            }
            var ticketID = new SqlParameter("@TicketID", id);
            _db.Database.ExecuteSqlCommand(cancelQuery, ticketID);

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public IActionResult Paycheck()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<IActionResult> Paycheck(DateTime? date)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var beginDate = date.Value;
            var endDate = date.Value.AddMonths(1);

            var sessions = (from info in _db.VBookingManagementPaycheck
                where info.CheckInDateTime >= beginDate && info.CheckInDateTime < endDate
                select new
                {
                    info.Name,
                    info.CheckInDateTime,
                    info.DeskId
                }).ToList();

            var model = new PaycheckOperationsViewModel
            {
                Items = (from info in sessions
                    group info by info.Name
                    into g
                    select new PaycheckOperationsViewItem
                    {
                        Name = g.Key,
                        CheckIns = g.Select(ig => new PaycheckOperationsCheckInInfo
                        {
                            CheckInDateTime = ig.CheckInDateTime.ToString("G"),
                            DeskId = ig.DeskId
                        }).ToList(),
                        Amount = g.Count().ToString()
                    }).OrderByDescending(i => i.Amount).ToList()
            };

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Management/PaycheckOperations", model) });
        }
    }
}