using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Data.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required]
        [StringLength(30, MinimumLength = 2)]
        [DisplayName("User Name")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string Password { get; set; }

        [NotMapped]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        [DisplayName("Confirm Password")]
        public string ConfirmPassword { get; set; }
        [DisplayName("Admin?")]
        public bool IsAdmin { get; set; }
        public List<UserRole> UserRoles { get; set; }
        //public List<Log> Logs { get; set; }
        //public List<SelectListItem> UserRolesDropdown { get; set; }

    }
}
