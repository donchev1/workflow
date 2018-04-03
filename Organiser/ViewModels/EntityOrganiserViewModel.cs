using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.ViewModels
{
    [Serializable]
    public class EntityOrganiserViewModel
    {

        public int LocStateNum { get; set; }
        public int OrderId { get; set; }
        public int EntitiesPassed { get; set; }
        public int LocStateId { get; set; }
        public int EntitiesInProgress { get; set; }
        public int SourceEntitiesReadyForCollection { get; set; }
    }

    [Serializable]
    public class PassEntitiesResultModel
    {

        public int LocStateNum { get; set; }
        public int OrderId { get; set; }
        public int EntitiesPassed { get; set; }
        public int TargetLocStateId { get; set; }
        public int TargetEntitiesInProgress { get; set; }
        public int SourceEntitiesReadyForCollection { get; set; }
        public string TargetLocStateStatus { get; set; }
    }


}
