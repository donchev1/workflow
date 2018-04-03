using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        public string ConfirmPassword { get; set; }
        public bool IsAdmin { get; set; }
        public List<UserRole> UserRoles { get; set; }
        //public List<Log> Logs { get; set; }
        [NotMapped]
        public List<SelectListItem> UserRolesDropdown { get; set; }

    }
}
