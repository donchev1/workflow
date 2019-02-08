using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using static Organiser.Controllers.HelperMethods;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Actions;
using Organiser.Actions.ActionObjects;

namespace Organiser.Controllers
{
    public class AccountController : Controller
    {
        private AppDbContext _appDbContext;
        private IUserRepository _userRepository;
        private IOrderRepository _orderRepository;
        private ILogRepository _logRepository;
        private IAccountActions _accountActions;
        public AccountController(
            AppDbContext appDbContext,
            IUserRepository userRepository,
            IOrderRepository orderRepository,
            ILogRepository logRepository,
            IAccountActions accountActions)
        {
            _appDbContext = appDbContext;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _logRepository = logRepository;
            _accountActions = accountActions;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (UserIsAdmin())
            {
                UsersViewModel model = new UsersViewModel();
                IEnumerable<User> users;
                users = _userRepository.GetUsers;
                foreach (User user in users)
                {
                    user.UserRolesDropdown = new List<SelectListItem>();
                    List<string> userStringRoles = new List<string>();
                    foreach (UserRole role in user.UserRoles)
                    {
                        userStringRoles.Add(((Locations)role.Role).ToString());
                    }
                    user.UserRoles = null;
                    user.UserRolesDropdown = DisplayUserRolesDropDown(userStringRoles);
                }

                return View(new UsersViewModel
                {
                    Users = users,
                });
            }
            else
            {
                return Error("You need to be logged in as admin to do this.");
            }
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Index", "Order");
            }
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model, string returnUrl = null)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            LoginActionObject loginActionObj = _accountActions.Login(model.UserName, model.Password);

            if (loginActionObj.UserExists)
            {
                if (model.RememberMe)
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginActionObj.ClaimsObject,
                    new AuthenticationProperties { IsPersistent = true });
                }
                else
                {
                    await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, loginActionObj.ClaimsObject,
                    new AuthenticationProperties { IsPersistent = false });
                }
                return RedirectToAction("Index", "Order");
            }

            ViewBag.errorMessage = "User name or password wrong. Please try again. *";
            return View();
        }

        [Authorize]
        [HttpGet]
        public IActionResult Create()
        {
            //TODO restrict to admin
            UsersCreateUpdateViewModel model = new UsersCreateUpdateViewModel();
            model.RoleDropDown = RoleDefaults();
            model.RoleDropDowns = RoleDropdownsWithSelectedRoles(new List<int>());
            model.Roles = Enumerable.Range(0, model.RoleDropDowns.Count).ToDictionary(x => x, x=>x);

            if (User.IsInRole("admin"))
            {
                return View(model);
            }
            else
            {
                return Error("You need to be logged in as admin to do this.");
            }
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Create(
        [Bind("UserEntity, Roles")] UsersCreateUpdateViewModel model)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            if (!ModelState.IsValid)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(roleOrganiser(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6 }));
                return View(model);
            }

            List<int> roleList = roleOrganiser(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6 });

            if (model.UserEntity.Password != model.UserEntity.ConfirmPassword)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(roleOrganiser(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6 }));
                ViewBag.errorMessage = "Password and Confirm Password fields must match!";
                return View(model);
            }
            else if (_userRepository.GetUserByName(model.UserEntity.UserName) != null)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(roleOrganiser(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6 }));
                ViewBag.errorMessage = "A user with the same user name already exists!";
                return View(model);
            }
            try
            {
                User user = new User();
                BuildUserEntity(model, ref user);

                if (roleList.Count > 0)
                {
                    user.UserRoles = CreateUserRoles(roleList);
                }
                _appDbContext.Add(user);
                await _appDbContext.SaveChangesAsync();

                _logRepository.CreateLog(
                  HttpContext.User.Identity.Name,
                  "Created a user. With user name: [" + user.UserName + "].",
                  DateTime.Now,
                  null);

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {

                ViewBag.ErrorMessage = ex;
                return View("Error");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string userName = HttpContext.User.Identity.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _logRepository.CreateLog(
                 userName,
                 "Logged out.",
                 DateTime.Now,
                 null);
            return Redirect("/Account/Login");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (!UserIsAdmin())
            {
                return Error("You have to be logged in as admin to do this.");
            }

            if (id == null)
            {
                return NotFound();
            }
            int idNotNull = (int)id;

            User user = _userRepository.GetUserAndRolesById(idNotNull);
            if (user == null)
            {
                return Error("User with id " + id.ToString() + " doesn't exist.");
            }

            UsersCreateUpdateViewModel model = BuildModelFromUser(user);
            model.RoleDropDowns = RoleDropdownsWithSelectedRoles(user.UserRoles.Select(x => x.Role).ToList());
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UserEntity, Role0, Role1, Role2, Role3, Role4, Role5, Role6, Role7")] UsersCreateUpdateViewModel model)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            if (model.UserEntity.Password != model.UserEntity.ConfirmPassword)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6, model.Role7 });
                ViewBag.errorMessage = "Password and Confirm Password fields must match!";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6, model.Role7 });
                return View(model);
            }

            if (!_userRepository.UserExists(model.UserEntity.UserId))
            {
                return Error("User with user id " + model.UserEntity.UserId.ToString() + " doesn't exist.");
            }
            if (_userRepository.GetUserByName(model.UserEntity.UserName) != null && _userRepository.GetUserByName(model.UserEntity.UserName).UserId != model.UserEntity.UserId)
            {
                model.RoleDropDowns = RoleDropdownsWithSelectedRoles(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3,
                model.Role4, model.Role5, model.Role6, model.Role7 });
                ViewBag.errorMessage = "A user with username " + model.UserEntity.UserName + " already exists.";
                return View(model);
            }
            User user = _userRepository.GetUserAndRolesById(model.UserEntity.UserId);
            BuildUserEntity(model, ref user);


            List<int> roleList = roleOrganiser(new List<int>() { model.Role0, model.Role1, model.Role2, model.Role3, model.Role4, model.Role5, model.Role6, model.Role7 });

            if (user.UserRoles.Count > 0)
            {
                _appDbContext.RemoveRange(user.UserRoles);
                await _appDbContext.SaveChangesAsync();
            }

            if (roleList.Count > 0)
            {
                user.UserRoles = CreateUserRoles(user, roleList.Distinct().ToList());
            }

            try
            {
                _appDbContext.Update(user);
                await _appDbContext.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_userRepository.UserExists(user.UserId))
                {
                    return Error("Something went wront. Try again.");
                }
                else
                {
                    throw;
                }
            }

            _logRepository.CreateLog(
                HttpContext.User.Identity.Name,
                "Edited user with user name: [" + user.UserName + "].",
                DateTime.Now,
                null);
            ViewBag.successMessage = "User details have been modified.";
            return RedirectToAction("Index");
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            User user = _userRepository.GetUserAndRolesById(id);
            if (user == null)
            {
                ViewBag.ErrorMessage = "User doesn't exist.";
                return View("Error");
            }

            List<string> stringRolesList = new List<string>();
            if (user.UserRoles != null && user.UserRoles.Count > 0)
            {
                foreach (UserRole role in user.UserRoles)
                {
                    stringRolesList.Add(((Locations)role.Role).ToString());
                }
            }

            return View(new UsersViewModel
            {
                UserDetails = user,
                UserRoles = stringRolesList
            });
        }

        [Authorize]
        [HttpGet]
        public IActionResult Delete(int id)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }
            var user = _userRepository.GetUserById(id);

            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        [Authorize]
        public IActionResult DeleteConfirmed(int id)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            if (_userRepository.UserExists(id))
            {

                var user = _appDbContext.Users.FirstOrDefault(u => u.UserId == id);
                if (HttpContext.User.Identity.Name == user.UserName)
                {
                    return Error("You can not delete your own user.");
                }
                _appDbContext.Users.Remove(user);
                _appDbContext.SaveChanges();
                TempData["success"] = "User (" + user.UserName + ") was successfully deleted!";

                _logRepository.CreateLog(
                HttpContext.User.Identity.Name,
                "Deleted a user. With user name: [" + user.UserName + "].",
                DateTime.Now,
                null);

                return RedirectToAction("Index");
            }
            else
            {
                return NotFound();
            }
        }

        private List<int> roleOrganiser(List<int> roleNum)
        {
            List<int> organisedList = new List<int>();
            for (int i = 0; i < roleNum.Count(); i++)
            {
                if (roleNum[i] != 0)
                {
                    if (!organisedList.Contains(roleNum[i]))
                    {
                        organisedList.Add(roleNum[i]);
                    }
                }
            }
            return organisedList;
        }

        private List<UserRole> CreateUserRoles(List<int> roleNums)
        {

            List<UserRole> roleList = new List<UserRole>();
            foreach (var role in roleNums)
            {
                roleList.Add(new UserRole { Role = role });
            }
            return roleList;
        }

        private bool UserIsAdmin()
        {
            string UserName = HttpContext.User.Identity.Name;
            return _userRepository.IsAdmin(UserName);
        }
        private void BuildUserEntity(UsersCreateUpdateViewModel model, ref User user)
        {
            user.UserId = model.UserEntity.UserId;
            user.UserName = model.UserEntity.UserName;
            user.Password = model.UserEntity.Password;
            user.ConfirmPassword = model.UserEntity.ConfirmPassword;
            user.IsAdmin = model.UserEntity.IsAdmin;
        }

        private UsersCreateUpdateViewModel BuildModelFromUser(User user)
        {
            return new UsersCreateUpdateViewModel()
            {
                UserEntity = new User()
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Password = user.Password,
                    ConfirmPassword = user.Password,
                    IsAdmin = user.IsAdmin
                }
            };
        }

        private List<UserRole> CreateUserRoles(User user, List<int> userRoles)
        {
            List<UserRole> newUserRoles = new List<UserRole>();
            foreach (int ur in userRoles)
            {
                newUserRoles.Add(new UserRole
                {
                    UserId = user.UserId,
                    Role = ur
                });
            }
            return newUserRoles;
        }
        public IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }

    }
}
