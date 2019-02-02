using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.ViewModels
{
    [Serializable]
    public class EntityOrganiserViewModel
    {

        public int DepartmentStateNum { get; set; }
        public int OrderId { get; set; }
        public int EntitiesPassed { get; set; }
        public int DepartmentStateId { get; set; }
        public int EntitiesInProgress { get; set; }
        public int SourceEntitiesRFC { get; set; }
    }

    [Serializable]
    public class PassEntitiesResultModel
    {
        public string StartTime { get; set; }
        public int DepartmentStateNum { get; set; }
        public int OrderId { get; set; }
        public int EntitiesPassed { get; set; }
        public int TargetDepartmentStateId { get; set; }
        public int TargetEntitiesInProgress { get; set; }
        public int SourceEntitiesRFC { get; set; }
        public string TargetDepartmentStateStatus { get; set; }
    }


}
