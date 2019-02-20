using Microsoft.EntityFrameworkCore;
using Organiser.Actions.ActionObjects;
using Organiser.Controllers;
using Organiser.Data.Models;
using Organiser.Data.UnitOfWork;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Actions
{
    public class LogActions : ILogActions
    {
        public IUnitOfWork _unitOfWork;

        public LogActions(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }
        public async Task<LogIndexActionObject> Index(string orderNumber, string userName, int? page)
        {
            using (_unitOfWork)
            {

                IQueryable<Log> Logs;
                LogIndexActionObject _actionObject = new LogIndexActionObject();

                int pageSize = 20;


                if (orderNumber != null)
                {
                    Logs = _unitOfWork.LogRepository.Find(x => x.OrderNumber == orderNumber);
                    if (Logs == null)
                    {
                        Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
                        _actionObject.Success = false;
                        _actionObject.ErrorMessage = "There are no event records related to order with order number: " + orderNumber;
                    }
                }
                else if (userName != null)
                {
                    Logs = _unitOfWork.LogRepository.Find(x => x.UserName == userName);
                    if (Logs == null)
                    {
                        Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
                        _actionObject.Success = false;
                        _actionObject.ErrorMessage = "There are no event records related to user with user name: " + userName;
                    }
                }
                else
                {
                    Logs = _unitOfWork.LogRepository.GetAllToIQuerable();
                }
                _actionObject.LogList = await PaginatedList<Log>.CreateAsync(Logs.AsNoTracking(), page ?? 1, pageSize);
                return _actionObject;
            }
        }

        public void EraseLogs(DateTime eraseTo)
        {
            using (_unitOfWork)
            {
                _unitOfWork.LogRepository.RemoveRange(x => x.CreatedAt < eraseTo);
            }
        }
    }
}