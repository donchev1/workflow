using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class Note
    {
        public int NoteId { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string Content { get; set; }
        public int Location { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Author { get; set; }
        [NotMapped]
        public string LocationName { get; set; }
    }
}
