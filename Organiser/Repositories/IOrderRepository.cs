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
        List<Order> GetAllActiveOrdersForLocation(int DepartmentStateType);
        IQueryable<Order> OrdersAndDepartmentStates(); 
        Order GetOrderById(int? OrderId);
        Order GetOrderAndDepartmentStatesById(int orderId);
        Order GetOrderByOrderNumber(string orderNubmer);
        IQueryable<Order> GetOrdersAndDepartmentStatesBySearchId(string SearchId);
        IQueryable<Order> GetAvailableOrdersForWork(int locationNameNum);
    }
}
