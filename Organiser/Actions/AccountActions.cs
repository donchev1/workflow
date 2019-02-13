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

        public IEnumerable<User> Index()
        {
            return _unitOfWork.UserRepository.GetAllUsersWithUserRoles();
        }

        public CreateActionObject CreatePost(string currentUserName, UsersCreateUpdateViewModel createViewModel)
        {
            using (_unitOfWork)
            {
                CreateActionObject _actionObject = new CreateActionObject();
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
                var user = _unitOfWork.UserRepository.GetUserByNameAndPassword(userName, password);
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
