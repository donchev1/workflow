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
        CreateObject CreatePost(string userName, UsersCreateUpdateViewModel createViewModel);
        UsersViewModel IndexAction();
        void Logout(string userName);
        User EditGet(int id);
        Task<ActionObject> EditPost(UsersCreateUpdateViewModel model, string currentUserName);
        User Details(int id);
        User DeleteGet(int id);
        ActionObject DeleteConfirmed(int userId, string currentUserName);
    }
}
