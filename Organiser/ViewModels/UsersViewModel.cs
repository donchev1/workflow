using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Organiser.Data.Models;


namespace Organiser.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<User> Users { get; set; }
        public User UserDetails { get; set; }

        public List<string> UserRoles { get; set; }

    }
}
