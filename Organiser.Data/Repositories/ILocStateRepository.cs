using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface IDepartmentStateRepository
    {
        DepartmentState GetDepartmentStateById(int DepartmentStateId);
        DepartmentState GetDepartmentStateByOrderIdAndLocNum(int OrderId, int LocNum);
        bool DepartmentStateExists(int id);
        bool IsLastOrderDepartmentState(int OrderId, int DepartmentStateId);
    }
}
