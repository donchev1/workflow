using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        [DisplayName("Order Number")]
        public string OrderNumber { get; set; }

        [StringLength(30)]
        [Required]
        [Display(Name = "Entity Type")]
        public string EntityType { get; set; }

        [Display(Name = "Entity Count")]
        [Range(0, 10000000, ErrorMessage = "Pick a number from 0 to 10000000")]
        public int EntityCount { get; set; }

        [Display(Name = "Entities In Progress")]
        public int EntitiesInProgress { get; set; }

        [Display(Name = "Entities Completed")]
        public int EntitiesCompleted { get; set; }

        [Display(Name = "Awaiting Processing")]
        public int EntitiesNotProcessed { get; set; }

        [StringLength(30)]
        public string Status { get; set; }

        [Display(Name = "Created at")]
        public DateTime CreatedAt { get; set; }
        [Display(Name = "Started At")]
        public DateTime StartedAt { get; set; }
        [Display(Name = "Finished at")]
        public DateTime FinshedAt { get; set; }
        [Display(Name = "Deadline date")]
        public DateTime DeadLineDate { get; set; }
        public List<DepartmentState> DepartmentStates { get; set; }
        public string Customer { get; set; }

        [NotMapped]
        public List<SelectListItem> StatusDefaultsDropdown { get; set; }

    }
}
