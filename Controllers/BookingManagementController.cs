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
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var airItems = new List<SalesViewItem>
            {
                new SalesViewItem
                {
                    Description = "Продажи",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                new SalesViewItem
                {
                    Description = "Штрафы",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                new SalesViewItem
                {
                    Description = "Добровольные обмены",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                new SalesViewItem
                {
                    Description = "Добровольные возвраты",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0,
                    SegCountCustomStyle = "color:red;",
                    AmountCashCustomStyle = "color:red;",
                    AmountPKCustomStyle = "color:red;",
                    AmountBNCustomStyle = "color:red;"
                },
                new SalesViewItem
                {
                    Description = "Вынужденные обмены",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0
                },
                new SalesViewItem
                {
                    Description = "Вынужденные возвраты",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0,
                    AmountCashCustomStyle = "color:red;",
                    AmountPKCustomStyle = "color:red;",
                    AmountBNCustomStyle = "color:red;"
                }
            };

            var airTotalItem = airItems.GroupBy(i => 1).Select(i => new SalesViewItem
            {
                Description = "Итого",
                AmountCash = i.Sum(x => x.AmountCash),
                AmountPK = i.Sum(x => x.AmountPK),
                AmountBN = i.Sum(x => x.AmountBN),
                SegCount = i.Sum(x => x.SegCount),
                SegCountCustomStyle = "font-weight:bold;",
                AmountCashCustomStyle = "font-weight:bold;",
                AmountPKCustomStyle = "font-weight:bold;",
                AmountBNCustomStyle = "font-weight:bold;",
                AmountTotalCustomStyle = "font-weight:bold;"
            }).FirstOrDefault();

            var railItems = new List<SalesViewItem>
            {
                new SalesViewItem
                {
                    Description = "Продажи ж/д",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0,
                    SegCountCustomStyle = "font-weight:bold;",
                    AmountCashCustomStyle = "font-weight:bold;",
                    AmountPKCustomStyle = "font-weight:bold;",
                    AmountBNCustomStyle = "font-weight:bold;",
                    AmountTotalCustomStyle = "font-weight:bold;"
                },
                new SalesViewItem
                {
                    Description = "Возвраты ж/д",
                    AmountCash = 0,
                    AmountPK = 0,
                    AmountBN = 0,
                    SegCount = 0,
                    SegCountCustomStyle = "font-weight:bold;",
                    AmountCashCustomStyle = "font-weight:bold;",
                    AmountPKCustomStyle = "font-weight:bold;",
                    AmountBNCustomStyle = "font-weight:bold;",
                    AmountTotalCustomStyle = "font-weight:bold;"
                }
            };

            var totalItem = new SalesViewItem
            {
                Description = "Общий итог",
                AmountCash = airTotalItem.AmountCash + railItems.Sum(i => i.AmountCash),
                AmountPK = airTotalItem.AmountPK + railItems.Sum(i => i.AmountPK),
                AmountBN = airTotalItem.AmountBN + railItems.Sum(i => i.AmountBN),
                SegCount = airTotalItem.SegCount + railItems.Sum(i => i.SegCount),
                SegCountCustomStyle = "font-weight:bold;",
                AmountCashCustomStyle = "font-weight:bold;",
                AmountPKCustomStyle = "font-weight:bold;",
                AmountBNCustomStyle = "font-weight:bold;",
                AmountTotalCustomStyle = "font-weight:bold;"
            };

            airItems.Add(airTotalItem);
            airItems.Add(new SalesViewItem(true));
            airItems.AddRange(railItems);
            airItems.Add(new SalesViewItem(true));
            airItems.Add(totalItem);

            var model = new SalesViewModel
            {
                Items = airItems
            };
            return PartialView(model);
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
                    children = deskGroups.Where(dg => dg.Name != "Все").OrderBy(dg => dg.Name)
                        .Select(dg => new DeskFilterItem
                        {
                            id = dg.BMDeskGroupId.ToString(),
                            text = dg.Name,
                            icon = "false",
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
                    text = d.Description + $"({d.DeskId})" ?? d.DeskId,
                    icon = "false",
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
                    children = sessions.OrderBy(s => s.Name)
                        .Select(s => new DeskFilterItem
                        {
                            id = s.SessionId.ToString(),
                            text = s.Name,
                            icon = "false"
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