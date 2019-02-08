using System;
using System.Linq;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class LogRepository : ILogRepository
    {
        private AppDbContext _context;
        public LogRepository(AppDbContext context)
        {
            _context = context;
        }
        public IQueryable<Log> GetAllLogs()
        {
            return _context.Logs
                .OrderByDescending(l => l.CreatedAt);
        }
    }

}
