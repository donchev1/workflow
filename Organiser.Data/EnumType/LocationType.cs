using System;
using System.Collections.Generic;
using System.Text;

namespace Organiser.Data.EnumType
{
    public sealed class LocationType : TypeSafeEnum
    {
        public static LocationType Folirung = new LocationType("Folirung",1);
        public static LocationType Handarbeit = new LocationType("Handarbeit", 2);
        public static LocationType Inkchet = new LocationType("Inkchet", 3);
        public static LocationType Falcen = new LocationType("Falcen", 4);
        public static LocationType Covertirung = new LocationType("Covertirung", 5);
        public static LocationType Lager = new LocationType("Lager", 6);
        public static LocationType Fahrer = new LocationType("Fahrer", 7);
        public static LocationType Orders = new LocationType("Orders", 8);
        public LocationType(string name, int value) : base(name, value)
        {
        }
    }
}
