using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{
    public class ActionObject
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public bool RedirectToError { get; set; }
    }
}
