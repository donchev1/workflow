using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static Organiser.Controllers.HelperMethods;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Actions;
using Organiser.Actions.ActionObjects;
using Organiser.Data.EnumType;
using Organiser.Data.UnitOfWork;

namespace Organiser.Controllers
{
    public class AccountController : Controller
    {
        private IAccountActions _accountActions;
        private IUnitOfWork _unitOfWork;
        public AccountController(IAccountActions accountActions, IUnitOfWork unitOfWork)
        {
            _accountActions = accountActions;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public IActionResult Index()
        {
            if (UserIsAdmin())
            {
                UsersViewModel model = new UsersViewModel();
                IEnumerable<User> users;
                users = _unitOfWork.UserRepository.GetAllUsersWithUserRoles();
                foreach (User user in users)
                {
                    user.UserRolesDropdown = new List<SelectListItem>();
                    List<string> userStringRoles = new List<string>();
                    foreach (UserRole role in user.UserRoles)
                    {
                        userStringRoles.Add(((Enums.Department)role.Role).ToString());
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

            model.Roles = Enumerable.Range(0, model.RoleDropDowns.Count).Select(x=> 0).ToList();

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
            using (_unitOfWork)
            {
                if (!UserIsAdmin())
                {
                    return Error("You need to be logged in as admin to do this.");
                }

                if (!ModelState.IsValid)
                {
                    model.RoleDropDown = RoleDefaults();
                    return View(model);
                }


                if (model.UserEntity.Password != model.UserEntity.ConfirmPassword)
                {
                    model.RoleDropDown = RoleDefaults();
                    ViewBag.errorMessage = "Password and Confirm Password fields must match!";
                    return View(model);
                }
                else if (_unitOfWork.UserRepository.GetUserByName(model.UserEntity.UserName) != null)
                {
                    model.RoleDropDown = RoleDefaults();
                    ViewBag.errorMessage = "A user with the same user name already exists!";
                    return View(model);
                }
                try
                {
                    var roleList = model.Roles.Where(x => x != null)?.Distinct()?.ToList();
                    User user = new User();
                    BuildUserEntity(model, ref user);

                    if (roleList.Count > 0)
                    {
                        user.UserRoles = CreateUserRoles(roleList);
                    }
                    _unitOfWork.UserRepository.Add(user);
                    await _unitOfWork.CompleteAsync();

                    _unitOfWork.LogRepository.CreateLog(
                      HttpContext.User.Identity.Name,
                      "Created a user. With user name: [" + user.UserName + "].",
                      DateTime.Now,
                      null);
                    TempData["success"] = "User created.";
                    return RedirectToAction("Index");
                }
                catch (Exception ex)
                {
                    ViewBag.ErrorMessage = ex;
                    return View("Error");
                }

            }        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string userName = HttpContext.User.Identity.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _unitOfWork.LogRepository.CreateLog(
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

            User user = _unitOfWork.UserRepository.GetUserAndRolesById(idNotNull);
            if (user == null)
            {
                return Error("User with id " + id.ToString() + " doesn't exist.");
            }

            UsersCreateUpdateViewModel model = BuildModelFromUser(user);
            return View(model);
        }

        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit([Bind("UserEntity, Roles")] UsersCreateUpdateViewModel model)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            if (model.UserEntity.Password != model.UserEntity.ConfirmPassword)
            {
                model.RoleDropDown = RoleDefaults();
                ViewBag.errorMessage = "Password and Confirm Password fields must match!";
                return View(model);
            }

            if (!ModelState.IsValid)
            {
                model.RoleDropDown = RoleDefaults();
                return View(model);
            }

            if (!_unitOfWork.UserRepository.UserExists(model.UserEntity.UserId))
            {
                return Error("User with user id " + model.UserEntity.UserId.ToString() + " doesn't exist.");
            }
            if (_unitOfWork.UserRepository.GetUserByName(model.UserEntity.UserName) != null && _unitOfWork.UserRepository.GetUserByName(model.UserEntity.UserName).UserId != model.UserEntity.UserId)
            {
                model.RoleDropDown = RoleDefaults();
                ViewBag.errorMessage = "A user with username " + model.UserEntity.UserName + " already exists.";
                return View(model);
            }
            User user = _unitOfWork.UserRepository.GetUserAndRolesById(model.UserEntity.UserId);
            BuildUserEntity(model, ref user);


            List<int> roleList = model.Roles.Where(x => x != 0).Distinct().ToList();

            if (user.UserRoles.Count > 0)
            {
                _unitOfWork.UserRoleRepository.RemoveRange(user.UserRoles);
               await _unitOfWork.CompleteAsync();
            }

            if (roleList.Count > 0)
            {
                user.UserRoles = CreateUserRoles(user, roleList);
            }

            try
            {
                _unitOfWork.Update(user);
                await _unitOfWork.CompleteAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!_unitOfWork.UserRepository.UserExists(user.UserId))
                {
                    return Error("Something went wront. Try again.");
                }
                else
                {
                    throw;
                }
            }

            _unitOfWork.LogRepository.CreateLog(
                HttpContext.User.Identity.Name,
                "Edited user with user name: [" + user.UserName + "].",
                DateTime.Now,
                null);
            ViewBag.successMessage = "User details have been modified.";
            return RedirectToAction("Index");
        }
        private void UpdateUserRoles(ref User user, List<int> updatedRoles)
        {
            for (int i = 0; i < updatedRoles.Count; i++)
            {
                if (updatedRoles.Count-1 < i)
                {
                    user.UserRoles[i] = null;
                }
                else if(user.UserRoles[i] != null)
                {
                    user.UserRoles[i].Role = updatedRoles[i];
                }
                else
                {
                    user.UserRoles.Add(new UserRole
                    {
                        Role = updatedRoles[i],
                        User = user
                    });
                }

            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult Details(int id)
        {
            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }

            User user = _unitOfWork.UserRepository.GetUserAndRolesById(id);
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
                    stringRolesList.Add(((Enums.Department)role.Role).ToString());
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
            var user = _unitOfWork.UserRepository.GetUserById(id);

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

            if (_unitOfWork.UserRepository.UserExists(id))
            {

                var user = _unitOfWork.UserRepository.Find(u => u.UserId == id).FirstOrDefault();
                if (HttpContext.User.Identity.Name == user.UserName)
                {
                    return Error("You can not delete your own user.");
                }
                _unitOfWork.UserRepository.Remove(user);
                _unitOfWork.Complete();
                TempData["success"] = "User (" + user.UserName + ") was successfully deleted!";

                _unitOfWork.LogRepository.CreateLog(
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
        [Obsolete]
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
            return _unitOfWork.UserRepository.IsAdmin(UserName);
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
            List<SelectListItem> _roleDefaults = RoleDefaults();
            List<int> _roles = user.UserRoles.Select(x => x.Role).ToList();
            _roles.AddRange(Enumerable.Range(1, _roleDefaults.Count - _roles.Count).Select(x => 0));
            return new UsersCreateUpdateViewModel()
            {
                UserEntity = new User()
                {
                    UserId = user.UserId,
                    UserName = user.UserName,
                    Password = user.Password,
                    ConfirmPassword = user.Password,
                    IsAdmin = user.IsAdmin
                },
                Roles = _roles,
                RoleDropDown = _roleDefaults
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
