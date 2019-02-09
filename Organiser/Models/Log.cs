using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class Log_Old
    {
        public int LogId { get; set; }

        [Column(TypeName = "ntext")]
        [Required]
        public string ActionRecord { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
    }
}
