using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Data.Models
{
    public class Note
    {
        public int NoteId { get; set; }

        public string Content { get; set; }
        public int Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }

        public string LocationName { get; set; }
    }
}
