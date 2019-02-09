using Microsoft.EntityFrameworkCore;
using Organiser.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class LogRepository : ILogRepository_Old
    {
        private readonly AppDbContext_Old _appDbContext;
        public IUserRepository _userRepository;
        public IOrderRepository _orderRepository;
        public LogRepository(
            AppDbContext_Old appDbContext,
            IUserRepository userRepository,
            IOrderRepository orderRepository)
        {
            _appDbContext = appDbContext;
            _userRepository = userRepository;
            _orderRepository = orderRepository;
        }
        public IQueryable<Log> GetAllLogs()
        {
            return _appDbContext.Logs
                .OrderByDescending(l => l.CreatedAt);

        }

        public IQueryable<Log> GetActionRecordsByOrderNumber(string orderNumber)
        {
            return _appDbContext.Logs
                .Where(l => l.OrderNumber.Contains(orderNumber))
                .OrderByDescending(l => l.CreatedAt);
        }

        public IQueryable<Log> GetActionRecordsByUserName(string userName)
        {
            return _appDbContext.Logs
                .Where(l => l.UserName.Contains(userName))
                .OrderByDescending(l => l.CreatedAt);
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

            _appDbContext.Add(newLog);
        }

        public void EraseLogsOlderThanDate(DateTime date)
        {
            _appDbContext.RemoveRange(_appDbContext.Logs.Where(n => n.CreatedAt < date));
            _appDbContext.SaveChanges();
        }
    }
}
