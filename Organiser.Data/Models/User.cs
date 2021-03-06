﻿using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace Organiser.Data.Models
{
    public class User
    {
        public int UserId { get; set; }

        public string UserName { get; set; }

        public string Password { get; set; }
        [NotMapped]
        public string ConfirmPassword { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserRole> UserRoles { get; set; }
        public List<Log> Logs { get; set; }

        [NotMapped]
        public List<SelectListItem> UserRolesDropdown { get; set; }

    }
}
