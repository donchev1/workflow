using Organiser.Actions.ActionObjects;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public interface IAccountActions
    {
        LoginActionObject Login(string userName, string password);
        CreateActionObject CreatePost(string userName, UsersCreateUpdateViewModel createViewModel);
        IEnumerable<User> Index();
    }
}
