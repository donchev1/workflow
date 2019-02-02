using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public interface IDepartmentStateRepository
    {
        DepartmentState GetDepartmentStateById(int DepartmentStateId);
        DepartmentState GetDepartmentStateByOrderIdAndLocNum(int OrderId, int LocNum);
        bool DepartmentStateExists(int id);
        bool IsLastOrderDepartmentState(int OrderId, int DepartmentStateId);
    }
}
