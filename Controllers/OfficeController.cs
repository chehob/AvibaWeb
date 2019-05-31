using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.OfficeViewModels;
using Microsoft.AspNetCore.Mvc;
using X.PagedList;

namespace AvibaWeb.Controllers
{
    public class OfficeController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private const int PageSize = 10;

        public OfficeController(AppIdentityDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return PartialView();
        }
    }
}