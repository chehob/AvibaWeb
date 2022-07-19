using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.KRSClientViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System;

namespace AvibaWeb.Controllers
{
    public class KRSClientController : Controller
    {
        private readonly AppIdentityDbContext _db;

        public KRSClientController(AppIdentityDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public async Task<IActionResult> Operations()
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = new OperationsViewModel
            {
                Items = await (from v in _db.VKRSTickets
                               where v.OperationDateTime >= DateTime.Today
                               orderby v.OperationId descending
                         select new OperationsViewItem
                         {
                             OperationId = v.OperationId,
                             TicketNumber = v.TicketNumber,
                             PassengerName = v.PassengerName,
                             SegCount = v.SegCount,
                             Payment = v.Payment.ToString("#,0.00", nfi),
                             KRSAmount = v.KRSAmount.ToString("#,0.00", nfi),
                             Total = (v.Payment + v.KRSAmount).ToString("#,0.00", nfi),
                             OperationTypeId = v.OperationTypeId
                         }).ToListAsync()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<IActionResult> GetNotificationData(int operationId)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var model = await (from v in _db.VKRSTicketNotificationData
                               where v.OperationId == operationId
                               select new TicketNotificationViewModel
                               {
                                   Uuid = v.Id.ToString(),
                                   TicketNumber = v.TicketNumber,
                                   PassengerName = v.PassengerName,
                                   Phone = v.Phone,
                                   OperationDateTime = v.OperationDateTime.ToString("dd.MM.yyyy hh:mm")
                               }).FirstOrDefaultAsync();

            return Json(model);
        }
    }
}