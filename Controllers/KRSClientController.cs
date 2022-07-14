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
            };

            return PartialView(model);
        }
    }
}