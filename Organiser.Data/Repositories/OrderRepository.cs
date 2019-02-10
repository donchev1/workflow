using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class OrderRepository : Repository<AppDbContext, Order>
    {
        public OrderRepository(AppDbContext context) : base(context)
        {
        }
    }
}
