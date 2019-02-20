using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Organiser.Data.UnitOfWork;
using Organiser.Data.Models;
using Organiser.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using Organiser.Data.EnumType;
using static Organiser.Controllers.HelperMethods;
using Organiser.Actions;
using Organiser.Actions.ActionObjects;

namespace Organiser.Controllers
{


    [Authorize]
    public class DepartmentStateController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IDepartmentStateActions _depStateActions;
        public DepartmentStateController(IDepartmentStateActions depStateActions, IUnitOfWork unitOfWork)

        {
            _depStateActions = depStateActions;
            _unitOfWork = unitOfWork;
        }

        [Authorize]
        public Dictionary<string, string> MarkAsReady(EntityOrganiserViewModel model)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();

            if (!ModelState.IsValid)
            {
                result.Add("MessageType", "Error");
                result.Add("Message", "Invalid selection. Please refresh the page and try again.");
                return result;
            }

            MarkAsReadyActionObject _actionObject = _depStateActions.MarkAsReady(model, User.Identity.Name);
            if (User.IsInRole(_actionObject.DepartmentState.Name))
            {
                Error("You don't have the necessary permissions to do that.");
            }

            return _actionObject.Result;
        }

        [Authorize]
        [HttpGet]
        public IActionResult MyWork(int DepartmentStateId, string errorType = "", string message = "", bool showMessages = false)
        {
            
            if (!User.IsInRole(((Enums.Department)DepartmentStateId).ToString()))
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

            List<Order> _orderList = _depStateActions.MyWork(DepartmentStateId);

            if (_orderList.Count() < 1)
            {
                return View(new OrderStateViewModel
                {
                    LocationName = ((Enums.Department)DepartmentStateId).ToString(),
                    LocationNameNum = GetLocationIntValue(((Enums.Department)DepartmentStateId).ToString()),
                    ShowMessages = showMessages
                });
            }

            return View(new OrderStateViewModel
            {
                LocationNameNum = GetLocationIntValue(_orderList[0].DepartmentStates[0].Name),
                LocationName = _orderList[0].DepartmentStates[0].Name,
                OrderList = _orderList,
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
