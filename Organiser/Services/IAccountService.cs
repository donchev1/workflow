using Organiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Services
{
    public interface IAccountService
    {
        User GetUserByNameAndPassword(string userName, string password);
        IEnumerable<int> GetUserRolesByUserId(int id);
    }
}
