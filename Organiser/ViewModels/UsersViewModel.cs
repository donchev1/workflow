using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Data.Models;


namespace Organiser.ViewModels
{
    public class UsersViewModel
    {
        public IEnumerable<UserViewModel> Users { get; set; }
        public UserViewModel UserDetails { get; set; }

        public List<string> UserRoles { get; set; }

    }

    public class UserViewModel
    {
        public int UserId { get; set; }

        [DisplayName("User Name")]
        public string UserName { get; set; }

        public string Password { get; set; }
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Is Admin")]
        public bool IsAdmin { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Log> Logs { get; set; }
        public List<SelectListItem> UserRolesDropdown { get; set; }

    }
}
