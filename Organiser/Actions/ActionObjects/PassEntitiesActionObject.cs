using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{
    public class PassEntitiesActionObject : ActionObject
    {
        public DepartmentState Source { get; set; }
        public DepartmentState Target { get; set; }
    }
}
