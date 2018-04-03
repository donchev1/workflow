using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class NewMessagesMonitor
    {
        public int NewMessagesMonitorId { get; set; }
        public bool Folirane  { get; set; }
        public bool ManualWork  { get; set; }
        public bool Falcing { get; set; }
        public bool InkChet  { get; set; }
        public bool Kovertirane  { get; set; }
        public bool Sklad  { get; set; }
        public bool Drivers  { get; set; }

    }
}
