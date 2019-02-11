using Organiser.Actions.ActionObjects;
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
        CreateActionObject Create(string userName, UsersCreateUpdateViewModel createViewModel);

    }
}
