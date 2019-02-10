using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Organiser.Data.Models
{
    public class Order
    {
        public int OrderId { get; set; }

        public string OrderNumber { get; set; }

        public string EntityType { get; set; }

        public int EntityCount { get; set; }

        public int EntitiesInProgress { get; set; }

        public int EntitiesCompleted { get; set; }

        public int EntitiesNotProcessed { get; set; }

        public string Status { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime FinshedAt { get; set; }
        public DateTime DeadLineDate { get; set; }
        public List<DepartmentState> DepartmentStates { get; set; }
        public string Customer { get; set; }

        [NotMapped]
        public List<SelectListItem> StatusDefaultsDropdown { get; set; }

    }
}
