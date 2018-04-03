using Organiser.Controllers;
using Organiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.ViewModels
{
    public class MessagesViewModel
    {
        public PaginatedList<Note> Notes { get; set; }
        public string LocationName { get; set; }
        public int LocationNameNum { get; set; }
        public bool userIsAdmin { get; set; }

    }
}
