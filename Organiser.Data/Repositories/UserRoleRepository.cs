using System;
using System.Collections.Generic;
using System.Text;
using Organiser.Data.Context;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class UserRoleRepository : Repository<AppDbContext, UserRole>
    {
        public UserRoleRepository(AppDbContext context) : base(context)
        {
        }
    }
}
