using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;


namespace Organiser.Models
{
    public class UserRepository : IUserRepository
    {
        public AppDbContext _appDbContext;

        public UserRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public bool UserExists(int id)
        {
            return _appDbContext.Users.Any(e => e.UserId == id);
        }

        public IEnumerable<User> Users
        {
            get { return _appDbContext.Users.Include(x => x.UserRoles).ToList(); }
        }

        public User GetUserById(int UserId)
        {
            return _appDbContext.Users.FirstOrDefault(u => u.UserId == UserId);
        }

        public User GetUserByName(string userName)
        {
            //returns null if it doesn't find any users
            return _appDbContext.Users.FirstOrDefault(u => u.UserName == userName);
        }

        public List<int> GetUserRolesByUserName(string name)
        {
            //returns an empty list if no matching id is found.
            
            User user = _appDbContext.Users.FirstOrDefault(u => u.UserName == name);
            return _appDbContext.UserRoles
                .Where(ur => ur.UserId == user.UserId).ToList().Select(ur => ur.Role).ToList();
        }

        public List<int> GetUserRolesByUserId(int id)
        {
            //returns an empty list if no matching id is found.
            return _appDbContext.UserRoles.Where(ur => ur.UserId == id).Select(ur => ur.Role).ToList();
        }

        public bool IsAdmin(string userName)
        {
            bool? isAdmin = _appDbContext.Users
                .Where(u => u.UserName == userName)
                .Select(u => u.IsAdmin).First();

            if (isAdmin != null)
            {
                bool isAdminbool = Convert.ToBoolean(isAdmin);
                return isAdminbool;
            }
            else
            {
                return false;
            }
        }
        

        public User GetUserAndRolesById(int userId)
        {
            return _appDbContext.Users
                .Include(user => user.UserRoles)
                .FirstOrDefault(u => u.UserId == userId);
        }


    }
}
