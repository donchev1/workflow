using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        [Required]
        [StringLength(50, MinimumLength = 2)]
        public string OrderNumber { get; set; }

        [StringLength(20)]
        [Display(Name = "Entity Type")]
        public string EntityType { get; set; }

        [Display(Name = "Entity Count")]
        public int EntityCount { get; set; }

        [Display(Name = "Entities In Progress")]
        public int EntitiesInProgress { get; set; }

        [Display(Name = "Entities Completed")]
        public int EntitiesCompleted { get; set; }

        [Display(Name = "Awaiting Processing")]
        public int EntitiesNotProcessed { get; set; }

        [StringLength(50)]
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinshedAt { get; set; }
        public List<LocState> LocStates { get; set; }

        [NotMapped]
        public List<SelectListItem> StatusDefaultsDropdown { get; set; }

        //public List<Log> Logs { get; set; }

    }
}
