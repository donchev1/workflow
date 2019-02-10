using System;
using System.Linq;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class LogRepository :  Repository<AppDbContext, Log>
    {
        public LogRepository(AppDbContext context) : base(context)
        {
        }

        public void CreateLog(string userName, string content, DateTime timeOfAction, string orderNumber = "")
        {
            Log newLog = new Log()
            {
                UserName = userName,
                ActionRecord = content,
                CreatedAt = timeOfAction,
                OrderNumber = orderNumber
            };

        }
    }

}
