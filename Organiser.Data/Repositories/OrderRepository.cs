using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.EnumType;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;

namespace Organiser.Data.Repositories
{
    public class OrderRepository : Repository<AppDbContext, Order>
    {
        public AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public IQueryable<Order> GetAvailableOrdersForWork(int locationNameNum)
        {
            string status = Enums.Statuses.InProgress.ToString();

            return _context.Orders.Where(o => o.Status == status || o.EntitiesNotProcessed > 0)
                .Include(o => o.DepartmentStates)
                .Where(o => (o.DepartmentStates.Any(ls =>
                                 ls.Name == ((Data.EnumType.Enums.Department)locationNameNum).ToString() &&
                                 o.EntitiesNotProcessed > 0 && o.DepartmentStates.Any(ls1 =>
                                     ls1.Name == ((Enums.Department)locationNameNum).ToString() &&
                                     ls1.LocationPosition == 1))
                             || o.DepartmentStates.Any(beforeLS =>
                                 beforeLS.LocationPosition == o.DepartmentStates.FirstOrDefault(originalLS =>
                                         originalLS.Name == ((Enums.Department)locationNameNum).ToString())
                                     .LocationPosition - 1 && beforeLS.EntitiesRFC > 0)));
        }

        public Order GetById(int id)
        {
            return _context.Orders.FirstOrDefault(x => x.OrderId == id);
        }

        public List<Order> GetAllActiveOrdersForLocation(int DepartmentStateType)
        {
            string departmentName = ((Enums.Department)DepartmentStateType).ToString();
            string status = Enums.Statuses.InProgress.ToString();
            var activeOrders = _context.Orders.Include(o => o.DepartmentStates)
                .Where(o => o.Status == status && o.DepartmentStates.Any(ls => ls.Name == departmentName && ls.EntitiesInProgress > 0)).ToList();

            foreach (Order o in activeOrders)
            {
                foreach (DepartmentState ls in o.DepartmentStates)
                {
                    o.DepartmentStates = o.DepartmentStates.Where(ls1 => ls1.Name == ((Enums.Department)DepartmentStateType).ToString() && ls1.EntitiesInProgress > 0).ToList();
                }
            }

            return activeOrders;
        }

        public Order GetOrderAndDepartmentStatesById(int orderId)
        {
            return _context.Orders
                .Include(order => order.DepartmentStates)
                .FirstOrDefault(o => o.OrderId == orderId);
        }

        public string GetOrderNumberByOrderId(int orderId)
        {

            return _context.Orders.Where(o => o.OrderId == orderId).Select(o => o.OrderNumber).FirstOrDefault();
        }
    }
}
