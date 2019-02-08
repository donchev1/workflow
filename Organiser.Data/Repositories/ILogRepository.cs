using System;
using System.Linq;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface ILogRepository
    {
        IQueryable<Log> GetAllLogs();
    }
}