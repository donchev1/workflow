using Organiser.Controllers;
using Organiser.Data.Models;

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
