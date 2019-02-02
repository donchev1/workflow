﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
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
    }
}