using System.Linq;
using AvibaWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace AvibaWeb.Controllers
{
    public class DirectoryController : Controller
    {
        private readonly AppIdentityDbContext _db;

        public DirectoryController(AppIdentityDbContext db)
        {
            _db = db;
        }

        // GET: Directory
        public ActionResult Index()
        {
            return PartialView();
        }

        [HttpGet]
        public ActionResult Staff()
        {
            var adminRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Administrators"));
            return PartialView(_db.Users.Where(u => u.IsActive && u.Roles.All(r => r.RoleId != adminRole.Id)).OrderBy(u=>u.Name));
        }
    }
}