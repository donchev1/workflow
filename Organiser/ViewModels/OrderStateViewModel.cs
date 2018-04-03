
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;
using Organiser.Controllers;
using Organiser.Models;


namespace Organiser.ViewModels
{
    [Serializable]
    public class OrderStateViewModel
    {
        public bool ShowMessages { get; set; }
        public int LocStateId { get; set; }
        public string LocationName { get; set; }
        public int LocationNameNum { get; set; }
        public PaginatedList<Order> OrderListPaginated { get; set; }
        public List<Order> OrderList { get; set; }

        public Order OrderDetails { get; set; }
        public string UserRole { get; set; }
        public int OrderId { get; set; }
        public int EntitiesPassed { get; set; }
        public List<int> AllowedPositions { get; set; }
        public Dictionary<string, string> FileNamesUrls { get; set; }
        public List<Note> Notes { get; set; }

    }
}
