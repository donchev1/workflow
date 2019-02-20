using Organiser.Actions.ActionObjects;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public interface IDepartmentStateActions
    {
        MarkAsReadyActionObject MarkAsReady(EntityOrganiserViewModel model, string currentUserName);

        List<Order> MyWork(int depStateId);

    }
}
