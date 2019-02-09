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
    }

}
