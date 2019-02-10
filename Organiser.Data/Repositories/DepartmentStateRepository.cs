using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.EntityFrameworkCore;
using Organiser.Data.Context;
using Organiser.Data.Migrations;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public class DepartmentStateRepository : Repository<AppDbContext, DepartmentState>
    {
        public AppDbContext _context;
        public DepartmentStateRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public bool DepartmentStateExists(int id)
        {
            return _context.DepartmentStates.Any(e => e.DepartmentStateId == id);
        }

        public DepartmentState GetDepartmentStateById(int DepartmentStateId)
        {
            return _context.DepartmentStates
                .Include(ls => ls.Order)
                .FirstOrDefault(ls => ls.DepartmentStateId == DepartmentStateId);
        }

        public DepartmentState GetDepartmentStateByOrderIdAndLocNum(int OrderId, int LocNum)
        {
            return _context.DepartmentStates.FirstOrDefault(ls => ls.Order.OrderId == OrderId && ls.LocationPosition == LocNum);
        }

        public bool IsLastOrderDepartmentState(int OrderId, int DepartmentStateId)
        {
            List<DepartmentState> DepartmentStates = _context.DepartmentStates
                .Where(ls => ls.OrderId == OrderId).ToList();

            if (DepartmentStates != null)
            {
                DepartmentState DepartmentState = DepartmentStates.OrderByDescending(ls => ls.LocationPosition).First();
                if (DepartmentState.DepartmentStateId == DepartmentStateId)
                {
                    return true;
                }
            }
            return false;
        }
    }
}
