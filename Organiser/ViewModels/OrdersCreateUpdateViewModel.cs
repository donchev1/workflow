using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Data.Models;


namespace Organiser.ViewModels
{
    public class OrdersCreateUpdateViewModel
    {
        public int OrderId { get; set; }
        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string OrderNumber { get; set; }
        [StringLength(20)]
        [Display(Name = "Entity Type")]
        public string EntityType { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        public int DepartmentState0 { get; set; }
        public int DepartmentState1 { get; set; }
        public int DepartmentState2 { get; set; }
        public int DepartmentState3 { get; set; }
        public int DepartmentState4 { get; set; }
        public int DepartmentState5 { get; set; }
        public int DepartmentState6 { get; set; }
        public int DepartmentState7 { get; set; }
        public List<List<SelectListItem>> DepartmentStatesDropDowns = new List<List<SelectListItem>>();

    }
}
