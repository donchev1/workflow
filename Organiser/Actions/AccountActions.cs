using Organiser.Actions.ActionObjects;
using Organiser.Controllers;
using Organiser.Data.UnitOfWork;
using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Organiser.Data.EnumType;
using System.Text;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Http;
using Organiser.ViewModels;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public class AccountActions : IAccountActions
    {
        public IUnitOfWork _unitOfWork;
        //to do - create a service for this 

        public AccountActions(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        private bool UserIsAdmin(string userName)
        {
            return _unitOfWork.UserRepository.IsAdmin(userName);
        }

        public User DeleteGet(int id)
        {
            return _unitOfWork.UserRepository.GetUserById(id);
        }

        public AccountActionObject DeleteConfirmed(int userId, string currentUserName)
        {
            AccountActionObject _actionObject = new AccountActionObject(); 
            if (_unitOfWork.UserRepository.UserExists(userId))
            {

                var user = _unitOfWork.UserRepository.Find(u => u.UserId == userId).FirstOrDefault();
                if (currentUserName == user.UserName)
                {
                    _actionObject.Success = false;
                    _actionObject.RedirectToError = true;
                    _actionObject.ErrorMessage ="You can not delete your own user.";
                }
                else
                {
                    _unitOfWork.UserRepository.Remove(user);
                    _unitOfWork.LogRepository.CreateLog(
                    currentUserName,
                    "Deleted a user. With user name: [" + user.UserName + "].",
                    DateTime.Now,
                    null);
                }

            }
            else
            {
                _actionObject.Success = false;
                _actionObject.RedirectToError = true;
                _actionObject.ErrorMessage = "User doesn't exist";
            }
            _unitOfWork.Complete();
            return _actionObject;
        }

        public void Logout(string userName)
        {
            _unitOfWork.LogRepository.CreateLog(
                userName,
                "Logged out.",
                DateTime.Now,
                null);
        }

        public User Details(int id)
        {
            return _unitOfWork.UserRepository.GetUserAndRolesById(id);

        }

        public User EditGet(int id)
        {
            return _unitOfWork.UserRepository.GetUserAndRolesById(id);
        }

        public async Task<AccountActionObject> EditPost(UsersCreateUpdateViewModel model, string currentUserName)
        {
            AccountActionObject _actionObject = new AccountActionObject();
            if (!_unitOfWork.UserRepository.UserExists(model.UserEntity.UserId))
            {
                _actionObject.Success = false;
                _actionObject.ErrorMessage = "User with user id " + model.UserEntity.UserId.ToString() + " doesn't exist. Please refresh the page.";
                return _actionObject;
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


            user.Password = Hash(user.Password);
            _unitOfWork.Update(user);

            _unitOfWork.LogRepository.CreateLog(
                currentUserName,
                "User edited  with user name: [" + user.UserName + "].",
                DateTime.Now,
                null);
            await _unitOfWork.CompleteAsync();

            return _actionObject;
        }

        public UsersViewModel IndexAction()
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
                user.UserRolesDropdown = HelperMethods.DisplayUserRolesDropDown(userStringRoles);
            }

            List<UserViewModel> _userViewModelList = new List<UserViewModel>();
            foreach (var user in users)
            {
                _userViewModelList.Add(Mapper.Map<UserViewModel>(user));
            }
            return new UsersViewModel
            {
                Users = _userViewModelList
            };
        }

        public CreateObject CreatePost(string currentUserName, UsersCreateUpdateViewModel createViewModel)
        {
            using (_unitOfWork)
            {
                CreateObject _actionObject = new CreateObject();
                _actionObject.UserIsAdmin = UserIsAdmin(currentUserName);
                if (!_actionObject.UserIsAdmin)
                {
                    return _actionObject;
                }

                if (_unitOfWork.UserRepository.GetUserByName(createViewModel.UserEntity.UserName) != null)
                {
                    _actionObject.UserCreated = false;
                    _actionObject.ErrorMessage = "A user with the same user name already exists!";
                    return _actionObject;
                }

                var roleList = createViewModel.Roles.Where(x => x != null)?.Distinct()?.ToList();
                User user = new User();
                BuildUserEntity(createViewModel, ref user);

                if (roleList.Count > 0)
                {
                    user.UserRoles = CreateUserRoles(user, roleList);
                }
                user.Password = Hash(user.Password);
                _unitOfWork.UserRepository.Add(user);

                _unitOfWork.LogRepository.CreateLog(
                  currentUserName,
                  "Created a user. With user name: [" + user.UserName + "].",
                  DateTime.Now,
                  null);
                _unitOfWork.Complete();

                _actionObject.UserCreated = true;
                return _actionObject;
            }
        }

        public LoginActionObject Login(string userName, string password)
        {
            using (_unitOfWork)
            {
                var user = _unitOfWork.UserRepository.GetUserByNameAndPassword(userName, Hash(password));
                bool userExists = user != null;
                ClaimsPrincipal principal = new ClaimsPrincipal();
                if (userExists)
                {
                    List<Claim> claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };

                    foreach (int role in _unitOfWork.UserRepository.GetUserRolesByUserId(user.UserId))
                    {
                        claims.Add(new Claim(ClaimTypes.Role, ((Enums.Department)role).ToString()));
                    }

                    if (user.IsAdmin)
                    {
                        claims.Add(new Claim(ClaimTypes.Role, "admin"));
                    }

                    ClaimsIdentity userIdentity = new ClaimsIdentity(claims, "login");
                    principal = new ClaimsPrincipal(userIdentity);

                    LoginActionObject loginActionObj = new LoginActionObject() { ClaimsObject = principal, UserExists = userExists };

                    _unitOfWork.LogRepository.CreateLog(
                      user.UserName,
                      "Logged in.",
                      DateTime.Now,
                      null);
                    _unitOfWork.Complete();

                }
                return new LoginActionObject { UserExists = userExists, ClaimsObject = principal };
            }
        }

        private string Hash(string inputString)
        {
            byte[] data = Encoding.ASCII.GetBytes(inputString);
            data = new SHA256Managed().ComputeHash(data);
            string hash = Encoding.ASCII.GetString(data);
            return hash;
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
        private void BuildUserEntity(UsersCreateUpdateViewModel model, ref User user)
        {
            user.UserId = model.UserEntity.UserId;
            user.UserName = model.UserEntity.UserName;
            user.Password = model.UserEntity.Password;
            user.ConfirmPassword = model.UserEntity.ConfirmPassword;
            user.IsAdmin = model.UserEntity.IsAdmin;
        }

      
    }
}
