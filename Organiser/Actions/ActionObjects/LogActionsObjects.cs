using Organiser.Controllers;
using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions.ActionObjects
{
    public class LogIndexActionObject : ActionObject
    {
        public PaginatedList<Log> LogList { get; set;}
    }
}
