using Microsoft.EntityFrameworkCore;
using Organiser.Controllers;
using Organiser.Data.EnumType;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public class OrderActions : IOrderActions
    {
        public IUnitOfWork _unitOfWork;

        public OrderActions(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<PaginatedList<Order>> Index(int? page, string SearchID = "")
        {
            using (_unitOfWork)
            {
                IQueryable<Order> orders;
                int pageSize = 15;
                if (SearchID != "" && SearchID != null)
                {

                    orders = _unitOfWork.OrderRepository.Find(o => o
                    .OrderNumber
                    .Contains(SearchID))
                    .Include(or => or.DepartmentStates)
                    .OrderByDescending(l => l.CreatedAt);

                    return await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize);
                }

                orders = _unitOfWork.OrderRepository.GetAllToIQuerable().Include(x => x.DepartmentStates)
                   .OrderByDescending(x => x.CreatedAt);
                return await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize);
            }
        }

        public async Task<OrderStateViewModel> AvailableWork(int locationNameNum, int? page)
        {
            using (_unitOfWork)
            {
                int pageSize = 15;
                IQueryable<Order> orders = _unitOfWork.OrderRepository.GetAvailableOrdersForWork(locationNameNum);
                return new OrderStateViewModel()
                {
                    LocationNameNum = locationNameNum,
                    LocationName = ((Enums.Department)locationNameNum).ToString(),
                    OrderListPaginated = await PaginatedList<Order>.CreateAsync(orders.AsNoTracking(), page ?? 1, pageSize)
                };
            }
        }

        public void CreatePost() { }
        public void EditGet() { }
        public void EditPost() { }
        public void PassEntities() { }
        public void DetailsGet() { }
        public void FirstPickUp() { }
        public void Delete() { }
        public void DeleteConfirmed() { }

    }
}
