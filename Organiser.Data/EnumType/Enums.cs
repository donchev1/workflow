using System;
using System.Collections.Generic;
using System.Text;

namespace Organiser.Data.EnumType
{
    public class Enums
    {
        public enum Statuses { NotStarted = 1, InProgress = 2, Finished = 3 }
        public enum Locations
        {
            Folirung = 1,
            Handarbeit = 2,
            Inkchet = 3,
            Falcen = 4,
            Covertirung = 5,
            Lager = 6,
            Fahrer = 7,
            Orders = 8,
        }
    }
}
