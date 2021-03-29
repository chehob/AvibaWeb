using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AvibaWeb.DomainModels;
using AvibaWeb.Infrastructure;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.HomeViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace AvibaWeb.Controllers
{
    public class HomeController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly IViewRenderService _viewRenderService;

        public HomeController(AppIdentityDbContext db, UserManager<AppUser> usrMgr,
             IViewRenderService viewRenderService)
        {
            _db = db;
            _userManager = usrMgr;
            _viewRenderService = viewRenderService;
        }

        [Authorize]
        public ViewResult Index()
        {
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> IncomingCollectionAlert()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var model = new IncomingCollectionAlert();
            if (user == null) return PartialView(model);

            model.IncomingNotAccepted =
                (from c in _db.Collections
                 join co in _db.CollectionOperations on c.CollectionId equals co.CollectionId into operations
                 from co in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                 where c.CollectorId == userId && co.OperationTypeId == CollectionOperationType.COType.New
                 select c.Amount).DefaultIfEmpty().Sum();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Home/IncomingCollectionAlert", model) });
        }
    }
}