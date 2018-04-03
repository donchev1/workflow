using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public interface IOrderRepository
    {
        string GetOrderNumberByOrderId(int orderId);
        List<Order> GetAllActiveOrdersForLocation(int locStateType);
        IEnumerable<Order> Orders { get; }
        IQueryable<Order> OrdersAndLocStates { get; }
        bool OrderExists(int id);
        Order GetOrderById(int? OrderId);
        Order GetOrderAndLocStatesById(int orderId);
        Order GetOrderByOrderNumber(string orderNubmer);
        IQueryable<Order> GetOrdersAndLocStatesBySearchId(string SearchId);
        IQueryable<Order> GetAvailableOrdersForWork(int locationNameNum);
        bool OrderExistsByOrderNumber(string orderNumber);
    }
}
