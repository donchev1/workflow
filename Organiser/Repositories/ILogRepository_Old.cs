using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public interface ILogRepository_Old
    {
        IQueryable<Log_Old> GetActionRecordsByUserName(string userName);
        IQueryable<Log_Old> GetAllLogs();
        IQueryable<Log_Old> GetActionRecordsByOrderNumber(string orderNumber);
        void CreateLog(string userName, string content, DateTime timeOfAction, string orderNumber = "");
        void EraseLogsOlderThanDate(DateTime date);
    }
}