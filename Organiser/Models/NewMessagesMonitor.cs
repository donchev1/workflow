using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class NewMessagesMonitor
    {
        public int NewMessagesMonitorId { get; set; }
        public bool Folirung { get; set; }
        public bool Handarbeit { get; set; }
        public bool Inkchet { get; set; }
        public bool Falcen { get; set; }
        public bool Covertirung { get; set; }
        public bool Lager { get; set; }
        public bool Fahrer { get; set; }

    }
}
