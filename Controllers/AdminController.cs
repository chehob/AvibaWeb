using System;
using AvibaWeb.Models;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using AvibaWeb.DomainModels;
using AvibaWeb.ViewModels.AdminViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Globalization;
using AvibaWeb.Infrastructure;
using AvibaWeb.ViewModels.CorpReceiptViewModels;

namespace AvibaWeb.Controllers
{
    public class AdminController : Controller
    {
        private readonly AppIdentityDbContext _db;
        private readonly UserManager<AppUser> _userManager;
        private readonly RoleManager<AppRole> _roleManager;
        private readonly IUserValidator<AppUser> _userValidator;
        private readonly IPasswordValidator<AppUser> _passwordValidator;
        private readonly IPasswordHasher<AppUser> _passwordHasher;
        private readonly IViewRenderService _viewRenderService;

        public AdminController(AppIdentityDbContext db,
            UserManager<AppUser> usrMgr,
            RoleManager<AppRole> roleMgr,
            IUserValidator<AppUser> userValid,
            IPasswordValidator<AppUser> passValid,
            IPasswordHasher<AppUser> passHash,
            IViewRenderService viewRenderService)
        {
            _db = db;
            _userManager = usrMgr;
            _roleManager = roleMgr;
            _userValidator = userValid;
            _passwordValidator = passValid;
            _passwordHasher = passHash;
            _viewRenderService = viewRenderService;
        }

        public ActionResult Index()
        {
            return PartialView();
        }

        #region Users
        public ActionResult Users()
        {
            var adminRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Administrators"));
            return PartialView(_userManager.Users
                .Where(u => u.Roles.All(r => r.RoleId != adminRole.Id))
                .OrderByDescending(u => u.IsActive)
                .ThenBy(u => u.Name));
        }

        [Authorize(Roles = "Administrators")]
        public ActionResult CreateUser()
        {
            var model = new CreateViewModel
            {
            };

            return PartialView(model);
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<ActionResult> CreateUser(CreateViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new AppUser
                {
                    UserName = model.UserName,
                    Name = model.Name,
                    Position = model.Position,
                    PhoneNumber = model.PhoneNumber,
                    UserITN = model.ITN
                };

                if (model.Photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await model.Photo.CopyToAsync(memoryStream);
                        user.Photo = memoryStream.ToArray();
                    }
                }

                var result = await _userManager.CreateAsync(user, model.Password);
                if (result.Succeeded)
                {
                    //await SignInManager.SignInAsync(user, isPersistent: false, rememberBrowser: false);

                    // For more information on how to enable account confirmation and password reset please visit http://go.microsoft.com/fwlink/?LinkID=320771
                    // Send an email with this link
                    // string code = await UserManager.GenerateEmailConfirmationTokenAsync(user.Id);
                    // var callbackUrl = Url.Action("ConfirmEmail", "Account", new { userId = user.Id, code = code }, protocol: Request.Url.Scheme);
                    // await UserManager.SendEmailAsync(user.Id, "Confirm your account", "Please confirm your account by clicking <a href=\"" + callbackUrl + "\">here</a>");

                    return RedirectToAction("Users");
                }

                AddErrors(result);
            }
            
            return PartialView(model);
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<ActionResult> EditUser(string id)
        {
            var user = await (from u in _db.Users
                where u.Id == id
                select u).FirstOrDefaultAsync();

            if (user != null)
            {
                var viewModel = new EditViewModel(user)
                {
                };

                return PartialView(viewModel);
            }
            else
            {
                return RedirectToAction("Users");
            }
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> EditUser(EditViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await (from u in _db.Users
                    where u.Id == model.Id
                    select u).FirstOrDefaultAsync();
                if (user != null)
                {
                    user.UserName = model.UserName;
                    user.Name = model.Name;
                    user.Position = model.Position;
                    user.PhoneNumber = model.PhoneNumber;
                    user.IsActive = model.IsActive;
                    user.UserITN = model.ITN;

                    if (model.Photo != null)
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            await model.Photo.CopyToAsync(memoryStream);
                            user.Photo = memoryStream.ToArray();
                        }
                    }

                    var validUserName = await _userValidator.ValidateAsync(_userManager, user);
                    if (!validUserName.Succeeded)
                    {
                        AddErrors(validUserName);
                    }
                    IdentityResult validPass = null;
                    if (!string.IsNullOrEmpty(model.Password))
                    {
                        validPass = await _passwordValidator.ValidateAsync(_userManager, user, model.Password);
                        if (validPass.Succeeded)
                        {
                            user.PasswordHash =
                                _passwordHasher.HashPassword(user, model.Password);
                        }
                        else
                        {
                            AddErrors(validPass);
                        }
                    }

                    if ((validUserName.Succeeded && validPass == null) || (validUserName.Succeeded
                                                                           && model.Password != string.Empty &&
                                                                           validPass.Succeeded))
                    {
                        var result = await _userManager.UpdateAsync(user);
                        if (result.Succeeded)
                        {
                            if (user.BookingMappingId != null && model.Password != null && model.Password != string.Empty)
                            {
                                UpdateBookingUser(model.Password, user.BookingMappingId);
                            }

                            return RedirectToAction("Users");
                        }
                        else
                        {
                            AddErrors(result);
                        }
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Пользователь не найден");
                }
            }

            return PartialView(model);
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<IActionResult> UserPhoto(string id)
        {
            var user = await (from u in _db.Users
                where u.Id == id
                select u).FirstOrDefaultAsync();
            if (user == null)
            {
                return NotFound();
            }

            return File(user.Photo, "image/jpeg");
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> DeleteUser(string id)
        {
            AppUser user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                IdentityResult result = await _userManager.DeleteAsync(user);
                if (result.Succeeded)
                {
                    return RedirectToAction("Users");
                }
                else
                {
                    return PartialView("Error", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                return PartialView("Error", new string[] { "Пользователь не найден" });
            }
        }
        #endregion

        #region Roles
        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public ActionResult Roles()
        {
            return PartialView(_db.Roles.GroupJoin(_db.UserRoles, r => r.Id, u => u.RoleId,
                (role, userRoles) => new RoleViewModel
                {
                    Id = role.Id,
                    Name = role.Name,
                    Members = userRoles.Join(_db.Users, ur=>ur.UserId, u=>u.Id, (userRole, user) => user.Name)
                }));
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public ActionResult CreateRole()
        {
            return PartialView();
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> CreateRole([Required]string name)
        {
            if (ModelState.IsValid)
            {
                IdentityResult result = await _roleManager.CreateAsync(new AppRole(name));
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    AddErrors(result);
                }
            }
            return PartialView(name);
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<ActionResult> EditRole(string id)
        {
            //var adminRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Administrators"));
            var role = await _roleManager.FindByIdAsync(id);
            IEnumerable<AppUser> members = from u in _db.Users
                where u.Roles.Any(r => r.RoleId == role.Id)
                orderby u.Name
                select u;
            var nonMembers = (from u in _db.Users
                //where u.Roles.All(r => r.RoleId != adminRole.Id) && u.IsActive
                where u.IsActive
                orderby u.Name
                select u).AsEnumerable().Except(members);
            return PartialView(new RoleEditModel
            {
                Role = role,
                Members = members,
                NonMembers = nonMembers
            });
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> EditRole(RoleModificationModel model)
        {
            IdentityResult result;
            if (ModelState.IsValid)
            {
                foreach (string userId in model.IdsToAdd ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    var role = await _roleManager.FindByNameAsync(model.RoleName);
                    if (user != null && role != null)
                    {
                        _db.UserRoles.Add(new IdentityUserRole<string>()
                        {
                            RoleId = role.Id,
                            UserId = user.Id
                        });
                        await _db.SaveChangesAsync();
                        //result = _userManager.AddToRoleAsync(user, model.RoleName).Result;
                        //if (!result.Succeeded)
                        //{
                        //    return PartialView("Error", result.Errors.Select(e => e.Description));
                        //}
                    }

                    //if (model.RoleName == "Cashiers")
                    //{
                    //    Cashier cashier = await db.Cashiers.FindAsync(userId);
                    //    if (cashier == null)
                    //    {
                    //        cashier = new Cashier(userId);
                    //        db.Cashiers.Add(cashier);
                    //    }
                    //    else
                    //    {
                    //        cashier.IsActive = true;
                    //    }
                    //}
                }
                foreach (string userId in model.IdsToDelete ?? new string[] { })
                {
                    var user = await _userManager.FindByIdAsync(userId);
                    if (user != null)
                    {
                        result = await _userManager.RemoveFromRoleAsync(user, model.RoleName);
                        if (!result.Succeeded)
                        {
                            return PartialView("Error", result.Errors.Select(e => e.Description));
                        }
                    }

                    //if (model.RoleName == "Cashiers")
                    //{
                    //    Cashier cashier = await db.Cashiers.FindAsync(userId);
                    //    if (cashier != null)
                    //    {
                    //        cashier.IsActive = false;
                    //    }
                    //    else
                    //    {
                    //        // Error
                    //    }
                    //}
                }
                await _db.SaveChangesAsync();
                return RedirectToAction("Roles");
            }
            return PartialView("Error", new string[] { "Роль не найдена" });
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> DeleteRole(string id)
        {
            AppRole role = await _roleManager.FindByIdAsync(id);
            if (role != null)
            {
                IdentityResult result = await _roleManager.DeleteAsync(role);
                if (result.Succeeded)
                {
                    return RedirectToAction("Roles");
                }
                else
                {
                    return PartialView("Error", result.Errors.Select(e => e.Description));
                }
            }
            else
            {
                return PartialView("Error", new string[] { "Роль не найдена" });
            }
        }
        #endregion

        #region Cards
        [Authorize(Roles = "Administrators")]
        public ActionResult Cards()
        {
            return PartialView(_db.Cards.ToList());
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public ActionResult CreateCard()
        {
            return PartialView(
                new CreateCardViewModel { Users = new SelectList(_userManager.Users, "Id", "Name") }
            );
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> CreateCard(CreateCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                Card card = new Card
                {
                    Number = model.Number,
                    UserId = model.UserId
                };
                _db.Cards.Add(card);
                await _db.SaveChangesAsync();

                return RedirectToAction("Cards");
            }
            return PartialView(model);
        }

        [Authorize(Roles = "Administrators")]
        [HttpGet]
        public async Task<ActionResult> EditCard(int id)
        {
            Card card = await _db.Cards.FindAsync(id);
            if (card != null)
            {
                return PartialView(
                    new EditCardViewModel(
                        card,
                        new SelectList(_userManager.Users, "Id", "Name", card.UserId)
                    )
                );
            }
            else
            {
                return RedirectToAction("Cards");
            }
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> EditCard(EditCardViewModel model)
        {
            if (ModelState.IsValid)
            {
                Card card = await _db.Cards.FindAsync(model.Id);
                if (card != null)
                {
                    card.Number = model.Number;
                    card.UserId = model.UserId;
                    await _db.SaveChangesAsync();
                    
                    return RedirectToAction("Cards");
                }
                else
                {
                    ModelState.AddModelError("", "Карта не найдена");
                }
            }

            return PartialView(model);
        }

        [Authorize(Roles = "Administrators")]
        [HttpPost]
        public async Task<ActionResult> DeleteCard(int id)
        {
            Card card = await _db.Cards.FindAsync(id);
            if (card != null)
            {
                _db.Cards.Remove(card);
                await _db.SaveChangesAsync();

                return RedirectToAction("Cards");
            }
            else
            {
                return PartialView("Error", new string[] { "Карта не найдена" });
            }
        }
        #endregion

        #region DeskGroups
        [HttpGet]
        public ActionResult DeskGroups()
        {
            return PartialView(_db.DeskGroups.Include(g => g.Desks).OrderBy(g => g.Name));
        }

        [HttpGet]
        public ActionResult CreateDeskGroup()
        {
            var model = new DeskGroupViewModel
            {
                Desks = GetDesksInGroupList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> CreateDeskGroup(DeskGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var deskGroup = new DeskGroup
                {
                    Name = model.Name
                };

                if (model.DeskIds != null)
                {
                    foreach (var id in model.DeskIds)
                    {
                        var desk = _db.Desks.FirstOrDefault(d => d.DeskId == id);
                        if (desk == null) continue;
                        desk.Group = deskGroup;
                    }
                }

                _db.DeskGroups.Add(deskGroup);
                await _db.SaveChangesAsync();

                return RedirectToAction("DeskGroups");
            }

            model.Desks = GetDesksInGroupList();
            return PartialView(model);
        }

        [HttpGet]
        public ActionResult EditDeskGroup(int id)
        {
            var group = _db.DeskGroups.FirstOrDefault(g => g.DeskGroupId == id);
            if (group == null) return RedirectToAction("DeskGroups");

            var viewModel = new DeskGroupViewModel
            {
                Name = group.Name,
                IsActive = group.IsActive,
                Desks = GetDesksInGroupList(id)
            };

            return PartialView(viewModel);

        }

        [HttpPost]
        public async Task<ActionResult> EditDeskGroup(DeskGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = _db.DeskGroups.Include(g => g.Desks).FirstOrDefault(g => g.DeskGroupId == model.Id);
                if (group != null)
                {
                    group.Name = model.Name;
                    group.IsActive = model.IsActive;

                    if (model.DeskIds != null || group.Desks.Count > 0)
                    {
                        var viewModelDesks = new List<Desk>();
                        var groupOldDesks = group.Desks.ToList();

                        foreach (var id in model.DeskIds)
                        {
                            var desk = _db.Desks.FirstOrDefault(d => d.DeskId == id);
                            if (desk == null) continue;

                            group.Desks.Add(desk);
                            viewModelDesks.Add(desk);
                        }

                        var desksToRemove = groupOldDesks.Except(viewModelDesks);
                        foreach (var c in desksToRemove)
                        {
                            group.Desks.Remove(c);
                        }
                    }

                    await _db.SaveChangesAsync();
                    return RedirectToAction("DeskGroups");
                }

                ModelState.AddModelError("", "Подразделение не найдено");
            }

            model.Desks = GetDesksInGroupList(model.Id);
            return PartialView(model);
        }
        #endregion

        #region SubagentDesks
        [HttpGet]
        public ActionResult SubagentDesks()
        {
            return PartialView(_db.Counterparties.Include(c => c.SubagentData)
                .ThenInclude(sd => sd.SubagentDesks).ThenInclude(sdd => sdd.Desk)
                .Where(c => c.Type.Description == "Субагент Р").OrderBy(c => c.Name));
        }

        [HttpGet]
        public ActionResult EditSubagentDesks(string id)
        {
            var group = _db.Counterparties.FirstOrDefault(c => c.ITN == id);
            if (group == null) return RedirectToAction("SubagentDesks");

            var viewModel = new SubagentDesksViewModel
            {
                Name = group.Name,
                Desks = GetSubagentDesksList(id)
            };

            return PartialView(viewModel);

        }

        [HttpPost]
        public async Task<ActionResult> EditSubagentDesks(SubagentDesksViewModel model)
        {
            if (ModelState.IsValid)
            {
                var subagent = _db.Counterparties.Include(c => c.SubagentData)
                    .ThenInclude(sd => sd.SubagentDesks).FirstOrDefault(c => c.ITN == model.Id);
                if (subagent != null)
                {
                    subagent.Name = model.Name;

                    if (model.DeskIds != null || subagent.SubagentData.SubagentDesks?.Count > 0)
                    {
                        var viewModelDesks = new List<SubagentDesk>();
                        var groupOldDesks = subagent.SubagentData.SubagentDesks.ToList();

                        if (model.DeskIds != null)
                            foreach (var id in model.DeskIds)
                            {
                                var desk = new SubagentDesk
                                {
                                    Subagent = subagent.SubagentData,
                                    Desk = _db.Desks.FirstOrDefault(d => d.DeskId == id)
                                };
                                if (desk.Desk == null) continue;

                                subagent.SubagentData.SubagentDesks.Add(desk);
                                viewModelDesks.Add(desk);
                            }

                        var desksToRemove = groupOldDesks.Except(viewModelDesks);
                        foreach (var c in desksToRemove)
                        {
                            subagent.SubagentData.SubagentDesks.Remove(c);
                            _db.Entry(c).State = EntityState.Deleted;
                        }
                    }

                    await _db.SaveChangesAsync();
                    return RedirectToAction("SubagentDesks");
                }

                ModelState.AddModelError("", "Субагент не найден");
            }

            model.Desks = GetSubagentDesksList(model.Id);
            return PartialView(model);
        }
        #endregion

        #region ExpenditureObject
        [HttpGet]
        public ActionResult ExpenditureObjects()
        {
            return PartialView(_db.ExpenditureObjects.OrderBy(o => o.Description));
        }

        [HttpGet]
        public ActionResult CreateExpenditureObject()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CreateExpenditureObject(ExpenditureObject model)
        {
            if (!ModelState.IsValid) return PartialView(model);

            var expenditureObject = new ExpenditureObject
            {
                Description = model.Description
            };

            _db.ExpenditureObjects.Add(expenditureObject);
            await _db.SaveChangesAsync();

            return RedirectToAction("ExpenditureObjects");
        }

        [HttpGet]
        public ActionResult EditExpenditureObject(int id)
        {
            var expenditureObject = _db.ExpenditureObjects.FirstOrDefault(o => o.ExpenditureObjectId == id);
            if (expenditureObject == null) return RedirectToAction("ExpenditureObjects");

            return PartialView(expenditureObject);
        }

        [HttpPost]
        public async Task<ActionResult> EditExpenditureObject(ExpenditureObject model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var expenditureObject = _db.ExpenditureObjects.FirstOrDefault(o => o.ExpenditureObjectId == model.ExpenditureObjectId);
            if (expenditureObject != null)
            {
                expenditureObject.Description = model.Description;
                expenditureObject.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("ExpenditureObjects");
            }

            ModelState.AddModelError("", "Статья расхода не найдена");

            return PartialView(model);
        }
        #endregion

        #region ExpenditureType
        [HttpGet]
        public ActionResult ExpenditureTypes()
        {
            return PartialView(_db.ExpenditureTypes.OrderBy(t => t.Description));
        }

        [HttpGet]
        public ActionResult CreateExpenditureType()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CreateExpenditureType(ExpenditureType model)
        {
            if (!ModelState.IsValid) return PartialView(model);

            var expenditureType = new ExpenditureType
            {
                Description = model.Description
            };

            _db.ExpenditureTypes.Add(expenditureType);
            await _db.SaveChangesAsync();

            return RedirectToAction("ExpenditureTypes");
        }

        [HttpGet]
        public ActionResult EditExpenditureType(int id)
        {
            var expenditureType = _db.ExpenditureTypes.FirstOrDefault(o => o.ExpenditureTypeId == id);
            if (expenditureType == null) return RedirectToAction("ExpenditureTypes");

            return PartialView(expenditureType);
        }

        [HttpPost]
        public async Task<ActionResult> EditExpenditureType(ExpenditureType model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var expenditureType = _db.ExpenditureTypes.FirstOrDefault(o => o.ExpenditureTypeId == model.ExpenditureTypeId);
            if (expenditureType != null)
            {
                expenditureType.Description = model.Description;
                expenditureType.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("ExpenditureTypes");
            }

            ModelState.AddModelError("", "Тип расходной операции не найден");

            return PartialView(model);
        }
        #endregion

        #region Desks
        public ActionResult Desks()
        {
            return PartialView(_db.Desks.OrderByDescending(d => d.IsActive).ThenBy(d => d.DeskId));
        }

        [HttpGet]
        public ActionResult EditDesk(string id)
        {
            var desk = _db.Desks.FirstOrDefault(d => d.DeskId == id);
            if (desk == null) return RedirectToAction("Desks");

            return PartialView(desk);
        }

        [HttpPost]
        public async Task<ActionResult> EditDesk(Desk model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var desk = _db.Desks.FirstOrDefault(d => d.DeskId == model.DeskId);
            if (desk != null)
            {
                desk.Description = model.Description;
                desk.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                UpdateBookingDesk(desk);

                return RedirectToAction("Desks");
            }

            ModelState.AddModelError("", "Пульт не найден");

            return PartialView(model);
        }
        #endregion

        #region Counterparties
        public ActionResult Counterparties()
        {
            return PartialView(_db.Counterparties.OrderBy(c => c.Name));
        }

        [HttpGet]
        public ActionResult EditCounterparty(string id)
        {
            var counterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == id);
            if (counterparty == null) return RedirectToAction("Counterparties");

            var model = new CounterpartyEditModel(counterparty)
            {
                Types = from t in _db.CounterpartyTypes.Where(t => t.IsActive).OrderBy(t => t.Description)
                    select new SelectListItem
                    {
                        Value = t.CounterpartyTypeId.ToString(),
                        Text = t.Description
                    },
                DeskGroups = new SelectList(
                    (from d in _db.DeskGroups.Where(d => d.IsActive).OrderBy(d => d.Name)
                     select new
                     {
                         Id = d.DeskGroupId,
                         d.Name
                     }).ToList(),
                    "Id",
                    "Name"),
                ExpenditureObjects = new SelectList(
                    (from e in _db.ExpenditureObjects.Where(e => e.IsActive).OrderBy(e => e.Description)
                     select new
                     {
                         Id = e.ExpenditureObjectId,
                         Name = e.Description
                     }).ToList(),
                    "Id",
                    "Name")
            };
            return PartialView(model);
        }

        [HttpPost]
        public async Task<ActionResult> EditCounterparty(CounterpartyEditModel model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var counterparty = _db.Counterparties.FirstOrDefault(c => c.ITN == model.Item.ITN);
            if (counterparty != null)
            {
                counterparty.Name = model.Item.Name;
                counterparty.CorrespondentAccount = model.Item.CorrespondentAccount;
                counterparty.KPP = model.Item.KPP;
                counterparty.BIK = model.Item.BIK;
                counterparty.OGRN = model.Item.OGRN;
                counterparty.Phone = model.Item.Phone;
                counterparty.Address = model.Item.Address;
                counterparty.Email = model.Item.Email;
                counterparty.BankName = model.Item.BankName;
                counterparty.BankAccount = model.Item.BankAccount;
                counterparty.ManagementName = model.Item.ManagementName;
                counterparty.ManagementPosition = model.Item.ManagementPosition;
                counterparty.TypeId = model.Item.TypeId;
                counterparty.ExpenditureDeskGroupId = model.Item.ExpenditureDeskGroupId;
                counterparty.ExpenditureObjectId = model.Item.ExpenditureObjectId;

                if(counterparty.ExpenditureDeskGroupId != null && counterparty.ExpenditureObjectId != null)
                {
                    UpdateIncomingExpenditures(counterparty);
                }

                await _db.SaveChangesAsync();

                return RedirectToAction("Counterparties");
            }

            ModelState.AddModelError("", "Контрагент не найден");

            return PartialView(model);
        }

        private void UpdateIncomingExpenditures(Counterparty counterparty)
        {
            var incomingExpendituresList = (from ie in _db.IncomingExpenditures
                                            .Include(ie => ie.FinancialAccountOperation).Include(ie => ie.Expenditures)
                     where ie.FinancialAccountOperation.CounterpartyId == counterparty.ITN
                     select ie).ToList();

            var unprocessedList = incomingExpendituresList.Where(ie => ie.IsProcessed == false).ToList();
            var processedList = incomingExpendituresList
                .Where(ie => ie.IsProcessed && ie.Expenditures
                    .Any(iee => iee.ObjectId != counterparty.ExpenditureObjectId.Value ||
                        iee.DeskGroupId != counterparty.ExpenditureDeskGroupId.Value))
                .ToList();
            foreach (var incomingExpenditure in unprocessedList)
            {
                var expenditure = new Expenditure
                {
                    Name = incomingExpenditure.FinancialAccountOperation.Description,
                    Amount = incomingExpenditure.Amount.Value,
                    DeskGroupId = counterparty.ExpenditureDeskGroupId.Value,
                    TypeId = _db.ExpenditureTypes.FirstOrDefault(et => et.Description == "Расход").ExpenditureTypeId,
                    ObjectId = counterparty.ExpenditureObjectId.Value,
                    PaymentTypeId = PaymentTypes.Cashless,
                    IncomingExpenditure = incomingExpenditure
                };

                var operation = new ExpenditureOperation
                {
                    Expenditure = expenditure,
                    OperationDateTime = DateTime.Now,
                    OperationTypeId = ExpenditureOperation.EOType.New
                };

                incomingExpenditure.IsProcessed = true;

                _db.ExpenditureOperations.Add(operation);
            }

            foreach (var incomingExpenditure in processedList)
            foreach (var expenditure in incomingExpenditure.Expenditures)
            {
                expenditure.DeskGroupId = counterparty.ExpenditureDeskGroupId.Value;
                expenditure.ObjectId = counterparty.ExpenditureObjectId.Value;
            }
        }
        #endregion

        #region CounterpartyType
        [HttpGet]
        public ActionResult CounterpartyTypes()
        {
            return PartialView(_db.CounterpartyTypes.OrderBy(t => t.Description));
        }

        [HttpGet]
        public ActionResult CreateCounterpartyType()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CreateCounterpartyType(CounterpartyType model)
        {
            if (!ModelState.IsValid) return PartialView(model);

            var counterpartyType = new CounterpartyType
            {
                Description = model.Description
            };

            _db.CounterpartyTypes.Add(counterpartyType);
            await _db.SaveChangesAsync();

            return RedirectToAction("CounterpartyTypes");
        }

        [HttpGet]
        public ActionResult EditCounterpartyType(int id)
        {
            var counterpartyType = _db.CounterpartyTypes.FirstOrDefault(o => o.CounterpartyTypeId == id);
            if (counterpartyType == null) return RedirectToAction("CounterpartyTypes");

            return PartialView(counterpartyType);
        }

        [HttpPost]
        public async Task<ActionResult> EditCounterpartyType(CounterpartyType model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var counterpartyType = _db.CounterpartyTypes.FirstOrDefault(o => o.CounterpartyTypeId == model.CounterpartyTypeId);
            if (counterpartyType != null)
            {
                counterpartyType.Description = model.Description;
                counterpartyType.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("CounterpartyTypes");
            }

            ModelState.AddModelError("", "Тип расходной операции не найден");

            return PartialView(model);
        }
        #endregion

        #region Organizations
        [HttpGet]
        public ActionResult Organizations()
        {
            return PartialView(_db.Organizations.Include(o => o.Accounts).OrderBy(o => o.Description));
        }

        [HttpGet]
        public ActionResult CreateOrganization()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CreateOrganization(Organization model)
        {
            if (!ModelState.IsValid) return PartialView(model);

            var organization = new Organization
            {
                Description = model.Description
            };

            _db.Organizations.Add(organization);
            await _db.SaveChangesAsync();

            return RedirectToAction("Organizations");
        }

        [HttpGet]
        public ActionResult EditOrganization(int id)
        {
            var organization = _db.Organizations.FirstOrDefault(o => o.OrganizationId == id);
            if (organization == null) return RedirectToAction("Organizations");

            return PartialView(organization);
        }

        [HttpPost]
        public async Task<ActionResult> EditOrganization(Organization model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var organization = _db.Organizations.FirstOrDefault(o => o.OrganizationId == model.OrganizationId);
            if (organization != null)
            {
                organization.Description = model.Description;
                organization.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("Organizations");
            }

            ModelState.AddModelError("", "Организация не найдена");

            return PartialView(model);
        }
        #endregion

        #region FinancialAccounts
        [HttpGet]
        public ActionResult AddFinancialAccount(int id)
        {
            return PartialView(new FinancialAccount
                {
                    OrganizationId = id
                });
        }

        [HttpPost]
        public async Task<ActionResult> AddFinancialAccount(FinancialAccount account)
        {
            if (!ModelState.IsValid) return PartialView(account);

            account.Organization = _db.Organizations.FirstOrDefault(o => o.OrganizationId == account.OrganizationId);
            _db.FinancialAccounts.Add(account);

            var initialOperation = new FinancialAccountOperation
            {
                Account = account,
                Amount = 0,
                OperationDateTime = DateTime.Now,
                InsertDateTime = DateTime.Now
            };

            _db.FinancialAccountOperations.Add(initialOperation);
            await _db.SaveChangesAsync();

            return RedirectToAction("Organizations");
        }

        [HttpGet]
        public ActionResult EditFinancialAccount(int id)
        {
            var account = _db.FinancialAccounts.FirstOrDefault(a => a.FinancialAccountId == id);
            if (account == null) return RedirectToAction("Organizations");

            return PartialView(account);
        }

        [HttpPost]
        public async Task<ActionResult> EditFinancialAccount(FinancialAccount model)
        {
            if (!ModelState.IsValid) return PartialView(model);
            var account = _db.FinancialAccounts.FirstOrDefault(a => a.FinancialAccountId == model.FinancialAccountId);
            if (account != null)
            {
                account.Description = model.Description;
                account.BankName = model.BankName;
                account.OffBankName = model.OffBankName;
                account.BIK = model.BIK;
                account.CorrespondentAccount = model.CorrespondentAccount;
                account.Balance = model.Balance;
                account.IsActive = model.IsActive;

                await _db.SaveChangesAsync();
                return RedirectToAction("Organizations");
            }

            ModelState.AddModelError("", "Расчетный счет не найден");

            return PartialView(model);
        }
        #endregion

        #region LoanGroups
        [HttpGet]
        public ActionResult LoanGroups()
        {
            return PartialView(_db.LoanGroups.Include(g => g.Counterparties).OrderBy(g => g.Description));
        }

        [HttpGet]
        public ActionResult CreateLoanGroup()
        {
            return PartialView();
        }

        [HttpPost]
        public async Task<ActionResult> CreateLoanGroup(LoanGroupViewModel model)
        {
            if (ModelState.IsValid)
            {
                var loanGroup = new LoanGroup
                {
                    Description = model.Name
                };

                _db.LoanGroups.Add(loanGroup);
                await _db.SaveChangesAsync();

                return RedirectToAction("LoanGroups");
            }

            return PartialView(model);
        }

        [HttpGet]
        public ActionResult EditLoanGroup(int id)
        {
            var group = _db.LoanGroups.FirstOrDefault(g => g.LoanGroupId == id);
            if (group == null) return RedirectToAction("LoanGroups");

            var viewModel = new LoanGroupViewModel
            {
                Id = id,
                Name = group.Description,
                IsActive = group.IsActive,
                Counterparties = _db.Counterparties.Select(c => c.Name).ToList(),
                GroupCounterparties = _db.Counterparties.Where(c => c.LoanGroupId == id).Select(c => c.Name).ToList(),
            };

            return PartialView(viewModel);

        }

        //[HttpPost]
        //public async Task<ActionResult> EditLoanGroup([FromBody] ShowCounterpartyOperationsViewModel model)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        var group = _db.LoanGroups.Include(g => g.Counterparties).FirstOrDefault(g => g.LoanGroupId == model.Id);
        //        if (group != null)
        //        {
        //            group.Description = model.Name;
        //            group.IsActive = model.IsActive;

        //            if (model.CounterpartyIds != null || group.Counterparties.Count > 0)
        //            {
        //                var viewModelCounterparties = new List<Counterparty>();
        //                var groupOldCounterparties = group.Counterparties.ToList();

        //                if (model.CounterpartyIds != null)
        //                    foreach (var id in model.CounterpartyIds)
        //                    {
        //                        var counterparty = _db.Counterparties.FirstOrDefault(d => d.ITN == id);
        //                        if (counterparty == null) continue;

        //                        group.Counterparties.Add(counterparty);
        //                        viewModelCounterparties.Add(counterparty);
        //                    }

        //                var counterpartiesToRemove = groupOldCounterparties.Except(viewModelCounterparties);
        //                foreach (var c in counterpartiesToRemove)
        //                {
        //                    group.Counterparties.Remove(c);
        //                }
        //            }

        //            await _db.SaveChangesAsync();
        //            return RedirectToAction("LoanGroups");
        //        }

        //        ModelState.AddModelError("", "Группа не найдена");
        //    }

        //    model.Counterparties = GetCounterpartiesInGroupList(model.Id);
        //    return PartialView(model);
        //}

        [HttpPost]
        public async Task<ActionResult> EditLoanGroup([FromBody] ShowCounterpartyOperationsViewModel model)
        {
            if (ModelState.IsValid)
            {
                var group = _db.LoanGroups.Include(g => g.Counterparties).FirstOrDefault(g => g.LoanGroupId == model.LoanGroupId);
                var counterparty = _db.Counterparties.FirstOrDefault(d => d.ITN == model.CounterpartyId);
                if (group != null && counterparty != null)
                {
                    foreach (var o in model.CounterpartyOperations)
                    {
                        if (o.IsProcessed)
                        {
                            group.Balance -= decimal.Parse(o.Amount.Replace(".", ",").Replace(" ", string.Empty));
                        }
                    }                    

                    group.Counterparties.Add(counterparty);

                    await _db.SaveChangesAsync();
                    return Json(new { loanGroupId = model.LoanGroupId });
                }

                ModelState.AddModelError("", "Группа не найдена");
            }

            return Json(new { loanGroupId = model.LoanGroupId });
        }

        [HttpGet]
        public ActionResult ShowCounterpartyOperations(int loanGroupId, string counterpartyName)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var counterpartyId = _db.Counterparties.FirstOrDefault(c => c.Name == counterpartyName).ITN;

            var model = new ShowCounterpartyOperationsViewModel
            {
                LoanGroupId = loanGroupId,
                CounterpartyId = counterpartyId,
                CounterpartyOperations = (from fao in _db.FinancialAccountOperations
                                          where fao.CounterpartyId == counterpartyId && fao.Amount >= 0
                                          select new CounterpartyOperation
                                          {
                                              OperationDate = fao.OperationDateTime.ToString("d"),
                                              PayeeName = fao.Account.Organization.Description,
                                              Amount = fao.Amount.ToString("#,0", nfi)
                                          }).ToList()
            };

            return PartialView(model);
        }
        #endregion

        #region CorporatorDocuments
        [HttpGet]
        public IActionResult CorporatorDocuments()
        {
            var model = new CorporatorDocumentsViewModel
            {
                Counterparties = (from c in _db.Counterparties
                    where c.Type.Description == "Корпоратор"
                    select c.Name).ToList(),
                Organizations = (from org in _db.Organizations
                    where org.IsActive
                    select org.Description).ToList()
            };

            return PartialView(model);
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentsByOrganization(string orgName)
        {
            var model = (from cd in _db.CorporatorDocuments
                join o in _db.Organizations on cd.OrganizationId equals o.OrganizationId
                where o.Description == orgName
                select new DocumentListViewModel
                {
                    CorporatorDocumentId = cd.CorporatorDocumentId,
                    Organization = cd.Corporator.Name,
                    Document = cd.Document,
                    Date = cd.Date
                }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Admin/DocumentList", model) });
        }

        [HttpGet]
        public async Task<IActionResult> GetDocumentsByCorporator(string payerName)
        {
            var model = (from cd in _db.CorporatorDocuments
                join c in _db.Counterparties on cd.ITN equals c.ITN
                where c.Name == payerName
                select new DocumentListViewModel
                {
                    CorporatorDocumentId = cd.CorporatorDocumentId,
                    Organization = cd.Organization.Description,
                    Document = cd.Document,
                    Date = cd.Date
                }).ToList();

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Admin/DocumentList", model) });
        }

        [HttpGet]
        public async Task<IActionResult> CreateDocument(string orgName, string corpName, int? id)
        {
            var model = new CreateDocumentViewModel
            {
                Counterparties = (from c in _db.Counterparties
                    where c.Type.Description == "Корпоратор"
                    select c.Name).ToList(),
                Organizations = (from org in _db.Organizations
                    where org.IsActive
                    select org.Description).ToList()
            };

            if (id != null && id != 0)
            {
                var doc = (from cd in _db.CorporatorDocuments.Include(c => c.Corporator).Include(c => c.Organization)
                    where cd.CorporatorDocumentId == id
                    select cd).FirstOrDefault();

                model.Document = new DocumentEditData
                {
                    DocumentId = doc.CorporatorDocumentId,
                    OrganizationName = doc.Organization.Description,
                    CorporatorName = doc.Corporator.Name,
                    Date = doc.Date.ToString("d"),
                    Doc = doc.Document
                };
            }
            else
            {
                model.Document = new DocumentEditData
                {
                    OrganizationName = orgName,
                    CorporatorName = corpName
                };
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Admin/CreateDocument", model) });
        }

        [HttpPost]
        public async Task<IActionResult> CreateDocument([FromBody]CreateDocumentPostViewModel model)
        {
            CorporatorDocument document;
            if (model.DocumentId != null && model.DocumentId != 0)
            {
                document = _db.CorporatorDocuments.FirstOrDefault(cr => cr.CorporatorDocumentId == model.DocumentId);
                document.ITN = (from c in _db.Counterparties
                    where c.Name == model.CorpName
                    select c.ITN).FirstOrDefault();
                document.Organization = (from o in _db.Organizations
                    where o.Description == model.OrgName
                    select o).FirstOrDefault();
                document.Document = model.Document;
                if (model.IssuedDateTime != null)
                {
                    document.Date = System.DateTime.Parse(model.IssuedDateTime);
                }
            }
            else
            {
                document = new CorporatorDocument
                {
                    ITN = (from c in _db.Counterparties
                        where c.Name == model.CorpName
                        select c.ITN).FirstOrDefault(),
                    Organization = (from o in _db.Organizations
                    where o.Description == model.OrgName
                    select o).FirstOrDefault(),
                    Document = model.Document,
                    Date = System.DateTime.Parse(model.IssuedDateTime)
                };

                _db.CorporatorDocuments.Add(document);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }

        [HttpGet]
        public async Task<IActionResult> EditDocumentTaxes(int id)
        {
            var model = new EditDocumentTaxesViewModel();

            var doc = (from cd in _db.CorporatorDocuments.Include(c => c.Corporator).Include(c => c.Organization)
                where cd.CorporatorDocumentId == id
                select cd).FirstOrDefault();

            if (doc == null)
            {
                
            }
            else
            {
                model.OrganizationId = doc.OrganizationId;
                model.CorporatorId = doc.ITN;

                var feeGroups = from cfr in _db.CorporatorFeeRates
                    join o in _db.Organizations on cfr.OrganizationId equals o.OrganizationId
                    join c in _db.Counterparties on cfr.ITN equals c.ITN
                    where cfr.ITN == doc.ITN && cfr.OrganizationId == doc.OrganizationId
                    group cfr by new { cfr.OperationTypeId, cfr.TicketTypeId }
                    into groups
                    select groups.OrderByDescending(p => p.StartDate).First();

                if (!feeGroups.Any())
                {
                    var feeRates = new List<CorpFeeListViewModel>();

                    var item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                        OperationType = CorpFeeListViewModel.CFOpType.Sale,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Sale,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                        OperationType = CorpFeeListViewModel.CFOpType.Refund,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Refund,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Avia,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Avia,
                        OperationType = CorpFeeListViewModel.CFOpType.Exchange,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Exchange,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                        OperationType = CorpFeeListViewModel.CFOpType.Sale,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Sale,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                        OperationType = CorpFeeListViewModel.CFOpType.Refund,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Refund,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    item = new CorpFeeListViewModel
                    {
                        TicketType = CorpFeeListViewModel.CFTicketType.Rail,
                        TicketTypeId = (int)CorpFeeListViewModel.CFTicketType.Rail,
                        OperationType = CorpFeeListViewModel.CFOpType.Exchange,
                        OperationTypeId = (int)CorpFeeListViewModel.CFOpType.Exchange,
                        Rate = 0,
                        PerSegment = true
                    };
                    feeRates.Add(item);

                    model.FeeRates = feeRates;
                }
                else
                {
                    model.FeeRates = (from fg in feeGroups.OrderBy(g => g.TicketTypeId).ThenBy(g => g.OperationTypeId)
                        select new CorpFeeListViewModel
                        {
                            TicketType = (CorpFeeListViewModel.CFTicketType) fg.TicketTypeId,
                            OperationType = (CorpFeeListViewModel.CFOpType) fg.OperationTypeId,
                            TicketTypeId = fg.TicketTypeId,
                            OperationTypeId = fg.OperationTypeId,
                            Rate = fg.Rate,
                            PerSegment = fg.PerSegment
                        }).ToList();
                }
            }

            return Json(new { message = await _viewRenderService.RenderToStringAsync("Admin/EditDocumentTaxes", model) });
        }

        [HttpPost]
        public async Task<IActionResult> EditDocumentTaxes([FromBody]EditDocumentTaxesViewModel model)
        {
            var feeGroups = (from cfr in _db.CorporatorFeeRates
                join o in _db.Organizations on cfr.OrganizationId equals o.OrganizationId
                join c in _db.Counterparties on cfr.ITN equals c.ITN
                where cfr.ITN == model.CorporatorId && cfr.OrganizationId == model.OrganizationId
                group cfr by new { cfr.OperationTypeId, cfr.TicketTypeId }
                into groups
                select groups.OrderByDescending(p => p.StartDate).First()).ToList();

            foreach (var feeRate in model.FeeRates)
            {
                var dbFeeRate = feeGroups.FirstOrDefault(cfr =>
                    cfr.OperationTypeId == feeRate.OperationTypeId && cfr.TicketTypeId == feeRate.TicketTypeId);

                if (dbFeeRate != null && dbFeeRate.Rate == feeRate.Rate && dbFeeRate.PerSegment == feeRate.PerSegment)
                {
                    continue;
                }

                var rate = new CorporatorFeeRate
                {
                    OrganizationId = model.OrganizationId,
                    ITN = model.CorporatorId,
                    OperationTypeId = feeRate.OperationTypeId,
                    TicketTypeId = feeRate.TicketTypeId,
                    Rate = feeRate.Rate,
                    PerSegment = feeRate.PerSegment,
                    StartDate = System.DateTime.Now
                };
                _db.CorporatorFeeRates.Add(rate);
            }

            await _db.SaveChangesAsync();

            return Json(new { message = "Ok" });
        }
        #endregion

        #region PKReceiptRules
        [HttpGet]
        public IActionResult PKReceiptRules()
        {
            var model = new PKReceiptRuleViewModel
            {
                Rules = (from p in _db.PKReceiptRules
                    select p.Rate).ToList()
            };

            return PartialView(model);
        }

        [HttpPost]
        public IActionResult AddPKReceiptRule([FromBody]AddPKReceiptRuleViewModel model)
        {
            var rule = (from p in _db.PKReceiptRules
                where p.Rate == model.Rate
                select p).FirstOrDefault();

            if (rule != null)
            {
                return Json(new { success = false, message = "" });
            }

            var newRule = new PKReceiptRule
            {
                Rate = model.Rate
            };

            _db.PKReceiptRules.Add(newRule);
            _db.SaveChanges();

            return Json(new { success = true, message = "" });
        }

        [HttpPost]
        public IActionResult RemovePKReceiptRule([FromBody]AddPKReceiptRuleViewModel model)
        {
            var rule = (from p in _db.PKReceiptRules
                where p.Rate == model.Rate
                select p).FirstOrDefault();

            if (rule == null)
            {
                return Json(new { success = false, message = "" });
            }

            _db.PKReceiptRules.Remove(rule);
            _db.SaveChanges();

            return Json(new { success = true, message = "" });
        }
        #endregion

        #region Helpers
        private void AddErrors(IdentityResult result)
        {
            foreach (var error in result.Errors)
            {
                ModelState.AddModelError("", error.Description);
            }
        }

        private async Task<MultiSelectList> GetAcceptedCollectorsList(string id = null)
        {
            var adminRole = _db.Roles.SingleOrDefault(r => r.Name.Contains("Administrators"));

            if (id == null)
            {
                return new MultiSelectList(_db.Users.Where(u => u.Roles.All(r => r.RoleId != adminRole.Id)), "Id", "Name");
            }

            var user = await ( from u in _db.Users
                               .Include(u => u.AcceptedCollectors)
                               where u.Id == id
                               select u).FirstOrDefaultAsync();
            if (user != null)
            {
                return new MultiSelectList(
                        _db.Users.Where(u => u.Id != id && u.Roles.All(r => r.RoleId != adminRole.Id)), "Id",
                        "Name", user.AcceptedCollectors.Select(u => u.CollectorId).ToList());
            }

            return new MultiSelectList(new List<string>());
        }

        private MultiSelectList GetDesksInGroupList(int? id = null)
        {
            var desks =
                _db.Desks
                    .Where(d => d.IsActive)
                    .Select(d => new
                    {
                        d.DeskId,
                        Description = $"{d.DeskId} {(string.IsNullOrEmpty(d.Description) ? "" : $"({d.Description})")}"
                    })
                    .ToList();

            if (id == null)
            {
                return new MultiSelectList(desks, "DeskId", "Description");
            }

            var group = _db.DeskGroups.Include(g => g.Desks).FirstOrDefault(g => g.DeskGroupId == id);
            if (group != null)
            {
                return new MultiSelectList(desks, "DeskId", "Description",
                    group.Desks.Select(u => u.DeskId).ToList());
            }

            return new MultiSelectList(new List<string>());
        }

        private MultiSelectList GetSubagentDesksList(string id = null)
        {
            var desks =
                _db.Desks
                    .Where(d => d.IsActive)
                    .Select(d => new
                    {
                        d.DeskId,
                        Description = $"{d.DeskId} {(string.IsNullOrEmpty(d.Description) ? "" : $"({d.Description})")}"
                    })
                    .ToList();

            if (id == null)
            {
                return new MultiSelectList(desks, "DeskId", "Description");
            }

            var group = _db.Counterparties.Include(c => c.SubagentData).ThenInclude(sd => sd.SubagentDesks)
                .FirstOrDefault(c => c.ITN == id);
            if (group != null)
            {
                return new MultiSelectList(desks, "DeskId", "Description",
                    group.SubagentData.SubagentDesks.Select(sd => sd.DeskId).ToList());
            }

            return new MultiSelectList(new List<string>());
        }

        private MultiSelectList GetCounterpartiesInGroupList(int? id = null)
        {
            if (id == null)
            {
                return new MultiSelectList(_db.Counterparties, "ITN", "Name");
            }

            var group = _db.LoanGroups.Include(g => g.Counterparties).FirstOrDefault(g => g.LoanGroupId == id);
            if (group != null)
            {
                return new MultiSelectList(_db.Counterparties, "ITN", "Name",
                    group.Counterparties.Select(c => c.ITN).ToList());
            }

            return new MultiSelectList(new List<string>());
        }

        private void UpdateBookingUser(string password, int? mappingId)
        {
            _db.Database.ExecuteSqlCommand(
                $@"UPDATE o
                SET o.Password = HASHBYTES('md5',cast({password} as varchar(40)))
                FROM [BookingDB].dbo.Operators2 o
                WHERE o.PersonID = {mappingId}"
            );
        }

        private void UpdateBookingDesk(Desk desk)
        {
            _db.Database.ExecuteSqlCommand(
                $@"UPDATE d
                SET d.IsActive = {desk.IsActive},
                    d.Description = {desk.Description}
                FROM [BookingDB].dbo.Desks d
                WHERE d.ID = {desk.DeskId}"
            );
        }
        #endregion
    }
}