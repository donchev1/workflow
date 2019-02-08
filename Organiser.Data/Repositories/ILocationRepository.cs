using System.Collections.Generic;
using Organiser.Data.Models;

namespace Organiser.Data.Repositories
{
    public interface ILocationRepository
    {
        List<Department> GetLocations(List<int> locationIds);
    }
}
