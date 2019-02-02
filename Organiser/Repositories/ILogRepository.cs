using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
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