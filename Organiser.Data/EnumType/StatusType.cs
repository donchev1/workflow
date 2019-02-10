using System;
using System.Collections.Generic;
using System.Text;

namespace Organiser.Data.EnumType
{
    public sealed class StatusType : TypeSafeEnum
    {
        public static readonly StatusType NotStarted = new StatusType("Not Started" , 1);
        public static readonly StatusType InProgress = new StatusType("In Progress" , 1);
        public static readonly StatusType Finished = new StatusType("Finished" , 1);

        public StatusType(string name, int value) : base(name,value)
        {
        }
    }
}
