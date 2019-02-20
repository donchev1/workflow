using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{


    public class CreateObject : ActionObject
    {
        public bool UserCreated { get; set; }
        public bool UserIsAdmin { get; set; }
    }

    public class LoginActionObject
    {
        public bool UserExists { get; set; }
        public ClaimsPrincipal ClaimsObject { get; set; }
    }
}
