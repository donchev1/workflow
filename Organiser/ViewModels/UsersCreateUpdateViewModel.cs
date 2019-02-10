using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Data.Models;


namespace Organiser.ViewModels
{
    public class UsersCreateUpdateViewModel
    {
        public User UserEntity { get; set; }
        public List<int> Roles { get; set; }
        public List<SelectListItem> RoleDropDown { get ;set;}
        public List<List<SelectListItem>> RoleDropDowns = new List<List<SelectListItem>>();

    }

}
