using System.IO;
using System.Threading.Tasks;
using AvibaWeb.DomainModels;
using AvibaWeb.Models;
using AvibaWeb.ViewModels.AccountViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace AvibaWeb.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly SignInManager<AppUser> _signInManager;
        private readonly IPasswordHasher<AppUser> _passwordHasher;

        public AccountController(AppIdentityDbContext db,
            UserManager<AppUser> usrMgr,
            SignInManager<AppUser> signInManager,
            IPasswordHasher<AppUser> passHash)
        {
            _db = db;
            _userManager = usrMgr;
            _signInManager = signInManager;
            _passwordHasher = passHash;
        }

        [AllowAnonymous]
        public ActionResult Login(string returnUrl)
        {
            if (ModelState.IsValid)
            {
            }

            ViewBag.returnUrl = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> Login(LoginModel details, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                //var user = await _userManager.FindByNameAsync(details.UserName) ?? 
                //           await CheckForBookingMapping(details.UserName, details.Password);

                var user = await _userManager.FindByNameAsync(details.UserName);
                if (user == null)
                {
                    user = await CheckForBookingMapping(details.UserName, details.Password);
                }

                if (user == null)
                {
                    ModelState.AddModelError("", "Неправильный логин/пароль.");
                }
                else if (user.IsActive == false)
                {
                    ModelState.AddModelError("", "Нет доступа.");
                }
                else
                {
                    await _signInManager.SignOutAsync();
                    var result = await _signInManager.PasswordSignInAsync(user, details.Password, false, false);
                    if (result.Succeeded)
                    {
                        return RedirectToLocal(returnUrl);
                    }
                }
            }
            ViewBag.returnUrl = returnUrl;
            return View(details);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Logout()
        {
            _signInManager.SignOutAsync();
            HttpContext.Session.Clear();
            return RedirectToAction("Login", "Account");
        }

        [HttpGet]
        public async Task<ActionResult> Profile(int pushalluserid)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return View();

            if (pushalluserid != 0)
            {
                user.PushAllUserId = pushalluserid;
                _db.Entry(user).Property(u => u.PushAllUserId).IsModified = true;
                await _db.SaveChangesAsync();
            }

            return PartialView(new ProfileViewModel
                {
                    PhoneNumber = user.PhoneNumber
                });
        }

        [HttpPost]
        public async Task<ActionResult> UpdateProfile(ProfileViewModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Json(new {status = "error"});

            if (model.Photo != null)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await model.Photo.CopyToAsync(memoryStream);
                    user.Photo = memoryStream.ToArray();
                }
                _db.Entry(user).Property(u => u.Photo).IsModified = true;
            }

            if (model.PhoneNumber != null)
            {
                user.PhoneNumber = model.PhoneNumber;
                _db.Entry(user).Property(u => u.PhoneNumber).IsModified = true;
            }

            await _db.SaveChangesAsync();

            return Json(new {status = "success"});
        }

        [HttpGet]
        public async Task<IActionResult> UserPhoto()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound();
            }

            return File(user.Photo, "image/jpeg");
        }

        #region Helpers
        private ActionResult RedirectToLocal(string returnUrl)
        {
            if (Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }
            return RedirectToAction("Index", "Home");
        }

        private async Task<AppUser> CheckForBookingMapping(string username, string password)
        {
            var user = await _userManager.FindByNameAsync(username);
            if (user.PasswordHash == null && user.BookingMappingId != null)
            {
                
                var bookingOperator = await _db.Query<BookingOperator>().FromSql(
                    $@"SELECT o.PersonID
                        FROM [BookingDB].dbo.Operators2 o
                        WHERE HASHBYTES('md5', '{password}' ) = o.Password").FirstOrDefaultAsync();
                if (user.BookingMappingId == bookingOperator.Id)
                {
                    user.PasswordHash = _passwordHasher.HashPassword(user, password);
                    var result = await _userManager.UpdateAsync(user);
                    if (!result.Succeeded)
                    {
                        user = null;
                    }
                }
                else
                {
                    user = null;
                }
            }
            else
            {
                user = null;
            }

            return user;
        }
        #endregion

    }
}