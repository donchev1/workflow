using Organiser.Actions.ActionObjects;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public interface ILogActions
    {
        void EraseLogs(DateTime eraseTo);
        Task<LogIndexActionObject> Index(string orderNumber, string userName, int? page);
    }
}
