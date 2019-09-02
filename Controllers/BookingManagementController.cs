using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.BookingManagement;

namespace AvibaWeb.Controllers
{
    public class BookingManagementController : Controller
    {
        private readonly AppIdentityDbContext _db;

        public BookingManagementController(AppIdentityDbContext db)
        {
            _db = db;
        }

        public IActionResult Index()
        {
            return PartialView();
        }

        public IActionResult Sales()
        {
            return PartialView();
        }

        public IActionResult Filter()
        {
            return PartialView();
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
                    @checked = false,
                    children = deskGroups.Where(dg => dg.Name != "Все").OrderBy(dg => dg.Name)
                        .Select(dg => new DeskFilterItem
                        {
                            id = dg.BMDeskGroupId.ToString(),
                            text = dg.Name,
                            @checked = false,
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
                    text = d.Description ?? d.DeskId,
                    @checked = false
                }).ToList();
        }
    }
}