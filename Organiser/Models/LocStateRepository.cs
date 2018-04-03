using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Models
{
    public class LocStateRepository : ILocStateRepository
    {
        private readonly AppDbContext _appDbContext;

        public LocStateRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }
        public bool LocStateExists(int id)
        {
            return _appDbContext.LocStates.Any(e => e.LocStateId == id);
        }

        public LocState GetLocStateById(int LocStateId)
        {
            return _appDbContext.LocStates
                .Include(ls => ls.Order)
                .FirstOrDefault(ls => ls.LocStateId == LocStateId);
        }

        public LocState GetLocStateByOrderIdAndLocNum(int OrderId, int LocNum)
        {
            return _appDbContext.LocStates.FirstOrDefault(ls => ls.Order.OrderId == OrderId && ls.LocationPosition == LocNum);
        }

        public bool IsLastOrderLocState(int OrderId, int LocStateId)
        {
            List<LocState> locStates = _appDbContext.LocStates
                 .Where(ls => ls.OrderId == OrderId).ToList();

            if (locStates != null)
            {
               LocState locState =  locStates.OrderByDescending(ls => ls.LocationPosition).First();
                if (locState.LocStateId == LocStateId)
                {
                    return true;
                }
            }
            return false;
        }

    }
}
