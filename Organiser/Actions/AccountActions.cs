using Organiser.Actions.ActionObjects;
using Organiser.Controllers;
using Organiser.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Data.Entity.Core.EntityClient;

namespace Organiser.Actions
{
    public class AccountActions : IAccountActions
    {
        public IAccountService _accountService;
        public AppDbContext _appDbContext;
        //to do - create a service for this 
        public ILogRepository _logRepository;

        public AccountActions(IAccountService accountService,
            AppDbContext appDbContext,
            ILogRepository logRepository)
        {
            _accountService = accountService;
            _appDbContext = appDbContext;
            _logRepository = logRepository;
        }

        public LoginActionObject Login(string userName, string password)
        {
            using (_appDbContext)
            {
                using (var dbContextTransaction = _appDbContext.Database.BeginTransaction())
                {
                    var user = _accountService.GetUserByNameAndPassword(userName, password);
                    bool userExists = user != null;
                    ClaimsPrincipal principal = new ClaimsPrincipal();
                    if (userExists)
                    {
                        var claims = new List<Claim> { new Claim(ClaimTypes.Name, user.UserName) };

                        foreach (int role in _accountService.GetUserRolesByUserId(user.UserId))
                        {
                            claims.Add(new Claim(ClaimTypes.Role, ((Locations)role).ToString()));
                        }

                        if (user.IsAdmin)
                        {
                            claims.Add(new Claim(ClaimTypes.Role, "admin"));
                        }

                        var userIdentity = new ClaimsIdentity(claims, "login");
                        principal = new ClaimsPrincipal(userIdentity);

                        var loginActionObj = new LoginActionObject() { ClaimsObject = principal, UserExists = userExists };

                        _logRepository.CreateLog(
                          user.UserName,
                          "Logged in.",
                          DateTime.Now,
                          null);
                        _appDbContext.SaveChanges();

                        dbContextTransaction.Commit();

                    }
                    return new LoginActionObject { UserExists = userExists, ClaimsObject = principal };
                }
            }

        }
    }
}
