using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class DepartmentStateRepository : IDepartmentStateRepository
    {
        private readonly AppDbContext _appDbContext;

        public DepartmentStateRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public bool DepartmentStateExists(int id)
        {
            return _appDbContext.DepartmentStates.Any(e => e.DepartmentStateId == id);
        }

        public DepartmentState GetDepartmentStateById(int DepartmentStateId)
        {
            return _appDbContext.DepartmentStates
                .Include(ls => ls.Order)
                .FirstOrDefault(ls => ls.DepartmentStateId == DepartmentStateId);
        }

        public DepartmentState GetDepartmentStateByOrderIdAndLocNum(int OrderId, int LocNum)
        {
            return _appDbContext.DepartmentStates.FirstOrDefault(ls => ls.Order.OrderId == OrderId && ls.LocationPosition == LocNum);
        }

        public bool IsLastOrderDepartmentState(int OrderId, int DepartmentStateId)
        {
            List<DepartmentState> DepartmentStates = _appDbContext.DepartmentStates
                 .Where(ls => ls.OrderId == OrderId).ToList();

            if (DepartmentStates != null)
            {
               DepartmentState DepartmentState =  DepartmentStates.OrderByDescending(ls => ls.LocationPosition).First();
                if (DepartmentState.DepartmentStateId == DepartmentStateId)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
