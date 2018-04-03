using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Models;


namespace Organiser.ViewModels
{
    public class UsersCreateUpdateViewModel
    {
        public int UserId { get; set; }
        [Required]
        [StringLength(30, MinimumLength = 2)]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string Password { get; set; }

        [DataType(DataType.Password)]
        [StringLength(25, MinimumLength = 6)]
        public string ConfirmPassword { get; set; }
        public bool IsAdmin { get; set; }
        public int Role0 { get; set; }
        public int Role1 { get; set; }
        public int Role2 { get; set; }
        public int Role3 { get; set; }
        public int Role4 { get; set; }
        public int Role5 { get; set; }
        public int Role6 { get; set; }
        public int Role7 { get; set; }
        public List<List<SelectListItem>> RoleDropDowns = new List<List<SelectListItem>>();

    }
}
