using Organiser.Actions.ActionObjects;
using Organiser.Controllers;
using Organiser.Data.UnitOfWork;
using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Organiser.Data.EnumType;

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
    }
}
