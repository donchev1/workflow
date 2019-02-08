using System.Collections.Generic;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface IUserRepository
    {
        bool UserExists(int id);
        List<int> GetUserRolesByUserId(int id);
        IEnumerable<User> GetUsers { get; }
        User GetUserById(int UserId);
        User GetUserByName(string userName);
        bool IsAdmin(string userName);
        User GetUserAndRolesById(int UserId);
        List<int> GetUserRolesByUserName(string name);
        bool HasRole(string userName, int roleNum);
        User GetUserByNameAndPassword(string userName, string password);
    }
}
