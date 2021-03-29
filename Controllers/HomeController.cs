using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using AvibaWeb.DomainModels;
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

        public HomeController(AppIdentityDbContext db, 
            UserManager<AppUser> usrMgr)
        {
            _db = db;
            _userManager = usrMgr;
        }

        [Authorize]
        public async Task<ViewResult> Index()
        {
            var userId = _userManager.GetUserId(User);
            var user = await _userManager.GetUserAsync(User);

            var model = new LayoutModel();
            if (user == null) return View(model);

            model.IncomingNotAccepted =
                (from c in _db.Collections
                 join co in _db.CollectionOperations on c.CollectionId equals co.CollectionId into operations
                 from co in operations.OrderByDescending(o => o.OperationDateTime).Take(1)
                 where c.CollectorId == userId && co.OperationTypeId == CollectionOperationType.COType.New
                 select c.Amount).DefaultIfEmpty().Sum();

            return View(model);
        }
    }
}