using Organiser.Actions.ActionObjects;
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
    public class DepartmentStateActions : IDepartmentStateActions
    {
        private readonly IUnitOfWork _unitOfWork;
        public DepartmentStateActions(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public MarkAsReadyActionObject MarkAsReady(EntityOrganiserViewModel model, string currentUserName)
        {
            using (_unitOfWork)
            {
                Dictionary<string, string> result = new Dictionary<string, string>();

                MarkAsReadyActionObject _actionResult = new MarkAsReadyActionObject();
                DepartmentState _departmentState = new DepartmentState();
                _departmentState = _unitOfWork.DepartmentStateRepository.GetDepartmentStateById(model.DepartmentStateId);

                var order = _unitOfWork.OrderRepository.GetById(_departmentState.OrderId);
                string additionalMessage = "";

                if (_departmentState.EntitiesInProgress < model.EntitiesPassed)
                {
                    _actionResult.Result.Add("MessageType", "Error");
                    _actionResult.Result.Add("Message", "There are only " + _departmentState.EntitiesInProgress.ToString() + " entities in progress. Refresh the page.");
                    return _actionResult;
                }

                _departmentState.EntitiesRFC += model.EntitiesPassed;
                _departmentState.EntitiesInProgress -= model.EntitiesPassed;
                _departmentState.EntitiesPassed += model.EntitiesPassed;

                if (_departmentState.TotalEntityCount == _departmentState.EntitiesPassed)
                {
                    _departmentState.Status = ((Enums.Statuses)3).ToString();
                    _departmentState.Finish = DateTime.Now;
                }

                //TODO test this if statement

                if (_unitOfWork.DepartmentStateRepository.IsLastOrderDepartmentState(_departmentState.OrderId, _departmentState.DepartmentStateId))
                {
                    order.EntitiesCompleted += model.EntitiesPassed;
                    if (_departmentState.EntitiesRFC == order.EntityCount)
                    {
                        order.FinshedAt = DateTime.Now;
                        order.Status = ((Enums.Statuses)3).ToString();
                        additionalMessage = "Order status: " + ((Enums.Statuses)3).ToString() + ".";
                    }
                    _unitOfWork.Update(order);


                }
                _unitOfWork.Update(_departmentState);
                _unitOfWork.Complete();

                result.Add("MessageType", "Success");

                result.Add("Message", model.EntitiesPassed.ToString() +
                    " entities successfully marked as ready for collection!" + additionalMessage);

                _unitOfWork.LogRepository.CreateLog(
                  currentUserName,
                  "Marked " + model.EntitiesPassed.ToString() + " " + order.EntityType + " as ready for collection.",
                  DateTime.Now,
                  order.OrderNumber);

                _actionResult.Result = result;
                _actionResult.DepartmentState = _departmentState;
                return _actionResult;

            }
        }
         
        public List<Order> MyWork(int depStateId)
        {
            using (_unitOfWork)
            {
                return _unitOfWork.OrderRepository.GetAllActiveOrdersForLocation(depStateId);
            }
        }

    }
}
