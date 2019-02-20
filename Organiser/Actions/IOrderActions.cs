using Organiser.Actions.ActionObjects;
using Organiser.Controllers;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public interface IOrderActions
    {
        Task<PaginatedList<Order>> Index(int? page, string SearchID = "");
        Task<OrderStateViewModel> AvailableWork(int locationNameNum, int? page);
        Task<ActionObject> CreatePost(Order order, string currentUserName);
        Task<Order> EditGet(int? id);
        ActionObject EditPost(Order order, string currentUserName));
    }
}
