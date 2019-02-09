using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Organiser.Data.Models
{
    public class Log
    {
        public int LogId { get; set; }

        public string ActionRecord { get; set; }
        public DateTime CreatedAt { get; set; }
        public string OrderNumber { get; set; }
        public string UserName { get; set; }
    }
}
