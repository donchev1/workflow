using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{
    public class CreateActionObject
    {
        public bool UserCreated { get; set; }
        public bool UserIsAdmin { get; set; }
        public string ErrorMessage { get; set; }
    }
}
