using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public interface ILocStateRepository
    {
        LocState GetLocStateById(int LocStateId);
        LocState GetLocStateByOrderIdAndLocNum(int OrderId, int LocNum);
        bool LocStateExists(int id);
        bool IsLastOrderLocState(int OrderId, int LocStateId);
    }
}
