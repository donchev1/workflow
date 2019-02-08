using System;
using System.Linq;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface ILogRepository
    {
        IQueryable<Log> GetActionRecordsByUserName(string userName);
        IQueryable<Log> GetAllLogs();
        IQueryable<Log> GetActionRecordsByOrderNumber(string orderNumber);
        void CreateLog(string userName, string content, DateTime timeOfAction, string orderNumber = "");
        void EraseLogsOlderThanDate(DateTime date);
    }
}