using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Actions;
using Organiser.Actions.ActionObjects;
using Organiser.Data.EnumType;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Controllers
{
    public class AccountController : Controller
    {
        private IAccountActions _accountActions;
        public AccountController(IAccountActions accountActions, IUnitOfWork unitOfWork)
        {
            _accountActions = accountActions;
        }

        [Authorize]
        public IActionResult Index()
        {
            UsersViewModel _viewModel = _accountActions.IndexAction();

            if (!UserIsAdmin())
            {
                return Error("You need to be logged in as admin to do this.");
            }
            return View(_viewModel);
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

            model.Roles = Enumerable.Range(0, model.RoleDropDown.Count).Select(x => 0).ToList();

            if (UserIsAdmin())
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

            string _userName = HttpContext.User.Identity.Name;
            CreateObject _actionObject = _accountActions.CreatePost(_userName, model);

            if (!_actionObject.UserIsAdmin)
            {
                return Error("You need to be logged in as admin to do this.");
            }
            else if (!_actionObject.UserCreated)
            {
                model.RoleDropDown = RoleDefaults();
                ViewBag.errorMessage = _actionObject.ErrorMessage;
                return View(model);
            }
            if (_actionObject.UserCreated)
            {
                TempData["success"] = "User created.";
                return RedirectToAction("Index");
            }
            else
            {
                TempData["error"] = "Oops, something went wrong, try again.";
                return RedirectToAction("Index");
            }
        }


        [HttpGet]
        public async Task<IActionResult> Logout()
        {
            string userName = HttpContext.User.Identity.Name;
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            _accountActions.Logout(userName);
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
            User user = _accountActions.EditGet(idNotNull);
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

            ActionObject _actionObject = await _accountActions.EditPost(model, User.Identity.Name);

            if(!_actionObject.Success)
            {
                return Error(_actionObject.ErrorMessage);
            }
           
            ViewBag.successMessage = "User details have been modified.";
            return RedirectToAction("Index");
        }
        private void UpdateUserRoles(ref User user, List<int> updatedRoles)
        {
            for (int i = 0; i < updatedRoles.Count; i++)
            {
                if (updatedRoles.Count - 1 < i)
                {
                    user.UserRoles[i] = null;
                }
                else if (user.UserRoles[i] != null)
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

            User user = _accountActions.Details(id);

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
                UserDetails = Mapper.Map<UserViewModel>(user),
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
            User user = _accountActions.DeleteGet(id);

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
            string _currentUserName = User.Identity.Name;
            ActionObject _actionObject = _accountActions.DeleteConfirmed(id, _currentUserName);
            if (!_actionObject.Success)
            {
                if(_actionObject.RedirectToError)
                {
                    return Error(_actionObject.ErrorMessage);
                }
                else
                {
                    ViewBag.ErrorMessage(_actionObject.ErrorMessage);
                }
            }
            TempData["success"] = "User (" + _currentUserName + ") was successfully deleted!";
            return RedirectToAction("Index");
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
            return User.IsInRole("admin");
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
        private string Hash(string inputString)
        {
            byte[] data = Encoding.ASCII.GetBytes(inputString);
            data = new SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
        }
    }
}
