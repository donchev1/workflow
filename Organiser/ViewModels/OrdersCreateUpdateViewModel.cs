using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Organiser.Models;


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
        public int LocState0 { get; set; }
        public int LocState1 { get; set; }
        public int LocState2 { get; set; }
        public int LocState3 { get; set; }
        public int LocState4 { get; set; }
        public int LocState5 { get; set; }
        public int LocState6 { get; set; }
        public int LocState7 { get; set; }
        public List<List<SelectListItem>> LocStatesDropDowns = new List<List<SelectListItem>>();

    }
}
