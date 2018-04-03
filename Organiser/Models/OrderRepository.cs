using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Organiser.Controllers;
using static Organiser.Controllers.HelperMethods;

namespace Organiser.Models
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _appDbContext;

        public OrderRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
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

        public IQueryable<Order> GetOrdersAndLocStatesBySearchId(string SearchId)
        { 
            return _appDbContext.Orders.Where(o => o.OrderNumber.Contains(SearchId))
                        .Include(order => order.LocStates)
                        .OrderByDescending(l => l.CreatedAt);
        }
        public IQueryable<Order> OrdersAndLocStates
        {
            get
            {
                return _appDbContext.Orders
                  .Include(order => order.LocStates);
            }
        }

        public Order GetOrderAndLocStatesById(int orderId)
        {
            return _appDbContext.Orders
                .Include(order => order.LocStates)
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
            return  _appDbContext.Orders.Where(o => o.Status == ((Statuses)2).ToString() || o.EntitiesNotProcessed > 0).Include(o => o.LocStates)
                .Where(o => (o.LocStates.Any(ls => ls.Name == ((Locations)locationNameNum).ToString()) && o.EntitiesNotProcessed > 0 && o.LocStates.Any(ls1 => ls1.Name == ((Locations)locationNameNum).ToString() && ls1.LocationPosition == 1))
                || o.LocStates.Any(beforeLS => beforeLS.LocationPosition == o.LocStates.FirstOrDefault(originalLS => originalLS.Name == ((Locations)locationNameNum).ToString()).LocationPosition - 1 && beforeLS.EntitiesReadyForCollection > 0));
        }

        public List<Order> GetAllActiveOrdersForLocation(int locStateType)
        {
            List<Order> activeOrders =  _appDbContext.Orders.Include(o => o.LocStates)
                .Where(o => o.Status == ((Statuses)2).ToString() && o.LocStates.Any(ls => ls.Name == ((Locations)locStateType).ToString() && ls.EntitiesInProgress > 0)).ToList();
            
            foreach (Order o in activeOrders)
            {
                foreach (LocState ls in o.LocStates)
                {
                    o.LocStates = o.LocStates.Where(ls1 => ls1.Name == ((Locations)locStateType).ToString() && ls1.EntitiesInProgress > 0).ToList();
                }
            }

            return activeOrders;
        }
    }
}
