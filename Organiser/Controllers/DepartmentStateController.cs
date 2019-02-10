﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organiser.Data.UnitOfWork;
using Organiser.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using static Organiser.Controllers.HelperMethods;
using DepartmentState = Organiser.Data.Models.DepartmentState;
using Order = Organiser.Data.Models.Order;

namespace Organiser.Controllers
{


    [Authorize]
    public class DepartmentStateController : Controller
    {
        private readonly INoteRepository _noteRepository;
        private readonly IUnitOfWork _unitOfWork;

        public DepartmentStateController(AppDbContext_Old context,
            IOrderRepository orderRepository,
            IUserRepository userRepository,
            IDepartmentStateRepository DepartmentStateRepository,
            INoteRepository noteRepository,
            ILogRepository_Old logRepository, IUnitOfWork unitOfWork)

        {
            _noteRepository = noteRepository;
            _unitOfWork = unitOfWork;

        }

        [Authorize]
        public Dictionary<string, string> MarkAsReady(EntityOrganiserViewModel model)
        {

            using (_unitOfWork)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                if (!ModelState.IsValid)
                {
                    result.Add("MessageType", "Error");
                    result.Add("Message", "Invalid selection. Please refresh the page and try again.");
                    return result;
                }



                try
                {
                    DepartmentState DepartmentState = _unitOfWork.DepartmentStateRepository.GetDepartmentStateById(model.DepartmentStateId);

                    if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, GetLocationIntValue(DepartmentState.Name)))
                    {
                        Error("Something went wrong, logout and log back in to fix the issue.");
                    }

                    Order order = _unitOfWork.OrderRepository.GetById(DepartmentState.OrderId);
                    string additionalMessage = "";

                    if (DepartmentState.EntitiesInProgress < model.EntitiesPassed)
                    {
                        result.Add("MessageType", "Error");
                        result.Add("Message", "There are only " + DepartmentState.EntitiesInProgress.ToString() + " entities in progress. Refresh the page.");
                        return result;
                    }

                    DepartmentState.EntitiesRFC += model.EntitiesPassed;
                    DepartmentState.EntitiesInProgress -= model.EntitiesPassed;
                    DepartmentState.EntitiesPassed += model.EntitiesPassed;

                    if (DepartmentState.TotalEntityCount == DepartmentState.EntitiesPassed)
                    {
                        DepartmentState.Status = ((Statuses_Old)3).ToString();
                        DepartmentState.Finish = DateTime.Now;
                    }

                    //TODO test this if statement

                    if (_unitOfWork.DepartmentStateRepository.IsLastOrderDepartmentState(DepartmentState.OrderId, DepartmentState.DepartmentStateId))
                    {
                        order.EntitiesCompleted += model.EntitiesPassed;
                        if (DepartmentState.EntitiesRFC == order.EntityCount)
                        {
                            order.FinshedAt = DateTime.Now;
                            order.Status = ((Statuses_Old)3).ToString();
                            additionalMessage = "Order status: " + ((Statuses_Old)3).ToString() + ".";
                        }
                        _unitOfWork.Update(order);


                    }
                    _unitOfWork.Update(DepartmentState);
                    _unitOfWork.Complete();

                    result.Add("MessageType", "Success");

                    result.Add("Message", model.EntitiesPassed.ToString() +
                        " entities successfully marked as ready for collection!" + additionalMessage);

                    _unitOfWork.LogRepository.CreateLog(
                      HttpContext.User.Identity.Name,
                      "Marked " + model.EntitiesPassed.ToString() + " " + order.EntityType + " as ready for collection.",
                      DateTime.Now,
                      order.OrderNumber);

                    return result;
                }
                catch (Exception)
                {
                    result.Add("MessageType", "Error");
                    result.Add("Message", "Invalid selection. Please refresh the page and try again.");
                    return result;
                }

            }
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyWork(int DepartmentStateId, string errorType = "", string message = "", bool showMessages = false)
        {
            string UserName = HttpContext.User.Identity.Name;
            List<int> userRoles = _unitOfWork.UserRepository.GetUserRolesByUserName(UserName);

            if (!_unitOfWork.UserRepository.HasRole(HttpContext.User.Identity.Name, DepartmentStateId))
            {
                return RedirectToAction("Logout", "Account");
            }

            if (errorType != "")
            {

                if (errorType == "Success")
                {
                    ViewBag.successMessage = message;
                }
                else
                {
                    ViewBag.errorMessage = message;
                }
            }


            List<Order> orderList = _unitOfWork.OrderRepository.GetAllActiveOrdersForLocation(DepartmentStateId);

            if (orderList.Count() < 1)
            {
                return View(new OrderStateViewModel
                {
                    LocationName = ((Locations_Old)DepartmentStateId).ToString(),
                    LocationNameNum = GetLocationIntValue(((Locations_Old)DepartmentStateId).ToString()),
                    ShowMessages = showMessages
                });
            }

            return View(new OrderStateViewModel
            {
                LocationNameNum = GetLocationIntValue(orderList[0].DepartmentStates[0].Name),
                LocationName = orderList[0].DepartmentStates[0].Name,
                OrderList = orderList,
                ShowMessages = showMessages
            });
        }

        private IActionResult Error(string errorMessage)
        {
            ViewBag.ErrorMessage = errorMessage;
            return View("Error");
        }
    }
}
