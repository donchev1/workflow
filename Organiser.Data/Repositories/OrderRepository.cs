using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class OrderRepository :  Repository<AppDbContext, Order>
    {
        private readonly AppDbContext _appDbContext;

        //public OrderRepository(AppDbContext appDbContext)
        //{
        //    _appDbContext = appDbContext;
        //}

        public OrderRepository(AppDbContext context) : base(context)
        {
        }
        public bool OrderExists(int id)
        {
            return _appDbContext.Orders.Any(e => e.OrderId == id);
        }

        public bool OrderExistsByOrderNumber(string orderNumber)
        {
            return _appDbContext.Orders.Any(e => e.OrderNumber == orderNumber);
        }
        public IEnumerable<Order> Orders
        {
            get { return _appDbContext.Orders; }
        }

        public IQueryable<Order> GetOrdersAndDepartmentStatesBySearchId(string SearchId)
        { 
            return _appDbContext.Orders.Where(o => o.OrderNumber.Contains(SearchId))
                        .Include(order => order.DepartmentStates)
                        .OrderByDescending(l => l.CreatedAt);
        }
        public IQueryable<Order> OrdersAndDepartmentStates
        {
            get
            {
                return _appDbContext.Orders
                  .Include(order => order.DepartmentStates)
                  .OrderByDescending(l => l.CreatedAt);
            }
        }

        public Order GetOrderAndDepartmentStatesById(int orderId)
        {
            return _appDbContext.Orders
                .Include(order => order.DepartmentStates)
                .FirstOrDefault(o => o.OrderId == orderId);
        }
        public Order GetOrderById(int? OrderId)
        {
            return _appDbContext.Orders.FirstOrDefault(o => o.OrderId == OrderId);
        }

        public Order GetOrderByOrderNumber(string orderNubmer)
        {
            return _appDbContext.Orders.FirstOrDefault(o => o.OrderNumber == orderNubmer);
        }

        public string GetOrderNumberByOrderId(int orderId)
        {

            return _appDbContext.Orders.Where(o => o.OrderId == orderId).Select(o => o.OrderNumber).FirstOrDefault(); 
        }

        public IQueryable<Order> GetAvailableOrdersForWork(int locationNameNum)
        {
            return  _appDbContext.Orders.Where(o => o.Status == ((EnumType.EnumType.Statuses)2).ToString() || o.EntitiesNotProcessed > 0).Include(o => o.DepartmentStates)
                .Where(o => (o.DepartmentStates.Any(ls => ls.Name == ((EnumType.EnumType.Locations)locationNameNum).ToString()) && o.EntitiesNotProcessed > 0 && o.DepartmentStates.Any(ls1 => ls1.Name == ((EnumType.EnumType.Locations)locationNameNum).ToString() && ls1.LocationPosition == 1))
                || o.DepartmentStates.Any(beforeLS => beforeLS.LocationPosition == o.DepartmentStates.FirstOrDefault(originalLS => originalLS.Name == ((EnumType.EnumType.Locations)locationNameNum).ToString()).LocationPosition - 1 && beforeLS.EntitiesRFC > 0));
        }

        public List<Order> GetAllActiveOrdersForLocation(int DepartmentStateType)
        {
            List<Order> activeOrders =  _appDbContext.Orders.Include(o => o.DepartmentStates)
                .Where(o => o.Status == ((EnumType.EnumType.Statuses)2).ToString() && o.DepartmentStates.Any(ls => ls.Name == ((EnumType.EnumType.Locations)DepartmentStateType).ToString() && ls.EntitiesInProgress > 0)).ToList();
            
            foreach (Order o in activeOrders)
            {
                foreach (DepartmentState ls in o.DepartmentStates)
                {
                    o.DepartmentStates = o.DepartmentStates.Where(ls1 => ls1.Name == ((EnumType.EnumType.Locations)DepartmentStateType).ToString() && ls1.EntitiesInProgress > 0).ToList();
                }
            }

            return activeOrders;
        }
    }
}
