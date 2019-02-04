using Organiser.Actions.ActionObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public interface IAccountActions
    {
        LoginActionObject Login(string userName, string password);
    }
}
