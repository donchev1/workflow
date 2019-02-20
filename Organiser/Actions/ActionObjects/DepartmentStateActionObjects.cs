using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{
  

    public class MarkAsReadyActionObject : ActionObject
    {
        public Dictionary<string, string> Result { get; set;}
        public DepartmentState DepartmentState { get ;set;}
    }
}
