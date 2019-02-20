using Microsoft.EntityFrameworkCore;
using Organiser.Actions.ActionObjects;
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

        public async Task<ActionObject> CreatePost(Order order, string currentUserName)
        {

            using (_unitOfWork)
            {
                ActionObject _actionObject = new ActionObject();
                if (_unitOfWork.OrderRepository.Find(e => e.OrderNumber == order.OrderNumber).Any())
                {
                    _actionObject.Success = false;
                    _actionObject.Message = "Error: order with order number " + order.OrderNumber + " already exists.";
                    return _actionObject;
                }

                order.CreatedAt = DateTime.Now;
                order.DepartmentStates = order.DepartmentStates.Where(x => x.NameNum != 0).ToList();
                List<DepartmentState> _departmentStates = new List<DepartmentState>();

                if (order.DepartmentStates.Any())
                {
                    _departmentStates = CreateOrderDepartmentStates(order);
                }
                else
                {
                    order.DepartmentStates.Add(new DepartmentState() { NameNum = 7 }); //drivers
                    _departmentStates = CreateOrderDepartmentStates(order);
                }

                order.EntitiesNotProcessed = order.EntityCount;
                order.DepartmentStates = _departmentStates;
                order.Status = ((Enums.Statuses)1).ToString();
                _unitOfWork.OrderRepository.Add(order);
                await _unitOfWork.CompleteAsync();

                _unitOfWork.LogRepository.CreateLog(
                    currentUserName,
                    "Order created.",
                    DateTime.Now,
                    order.OrderNumber);
                _actionObject.Message = "Order created successfully.";
                return _actionObject;
            }
        }
        public async Task<Order> EditGet(int? id)
        {
            return await _unitOfWork.OrderRepository.Find(x => x.OrderId == id).SingleOrDefaultAsync();
        }
        public ActionObject EditPost(Order order, string currentUserName)
        {
            using (_unitOfWork)
            {

                ActionObject _actionObject = new ActionObject();

                if (_unitOfWork.OrderRepository.Find(x => x.OrderNumber == order.OrderNumber).FirstOrDefault() is null
                && _unitOfWork.OrderRepository.Find(x => x.OrderNumber == order.OrderNumber).FirstOrDefault().OrderId != order.OrderId)
                {
                    _actionObject.Success = false;
                    _actionObject.Message = "Error: order with order number " + order.OrderNumber + " already exists.";
                    return _actionObject;
                }



                Order oldOrderState = _unitOfWork.OrderRepository.GetAllToIQuerable().Include(x => x.DepartmentStates)
                    .FirstOrDefault(o => o.OrderId == order.OrderId);

                if (order.Status != oldOrderState.Status)
                {
                    List<DepartmentState> newDepartmentStates = oldOrderState.DepartmentStates;
                    if (order.Status == ((Enums.Statuses)3).ToString())
                    {
                        if (order.StartedAt == DateTime.MinValue)
                        {
                            order.StartedAt = DateTime.Now;
                        }

                        order.FinshedAt = DateTime.Now;
                        foreach (DepartmentState ls in newDepartmentStates)
                        {
                            ls.Status = order.Status;
                            ls.EntitiesInProgress = 0;
                            ls.EntitiesRFC = 0;
                        }
                    }

                    oldOrderState.EntitiesNotProcessed = 0;
                    oldOrderState.DepartmentStates = newDepartmentStates;
                }

                UpdateOrderValues(ref oldOrderState, order);

                _unitOfWork.Update(oldOrderState);
                _unitOfWork.LogRepository.CreateLog(
                    currentUserName,
                    "Edited order.",
                    DateTime.Now,
                    order.OrderNumber);
                _unitOfWork.CompleteAsync();

                _actionObject.Message = "Order successfully updated.";
                return _actionObject;
            }
        }
        public void PassEntities() { }
        public void DetailsGet() { }
        public void FirstPickUp() { }
        public void Delete() { }
        public void DeleteConfirmed() { }

        private List<DepartmentState> CreateOrderDepartmentStates(Order order)
        {
            List<DepartmentState> orderNewDepartmentStates = new List<DepartmentState>();
            int count = 0;
            foreach (var ds in order.DepartmentStates)
            {
                ++count;
                orderNewDepartmentStates.Add(new DepartmentState
                {
                    Name = ((Enums.Department)ds.NameNum).ToString(),
                    LocationPosition = count,
                    Status = ((Enums.Statuses)1).ToString(),
                    TotalEntityCount = order.EntityCount
                });
            }
            return orderNewDepartmentStates;
        }
        private void UpdateOrderValues(ref Order order, Order newOrderDetails)
        {
            order.EntityType = newOrderDetails.EntityType;
            order.Status = newOrderDetails.Status;
            order.OrderNumber = newOrderDetails.OrderNumber;
        }

    }
}
