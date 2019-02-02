using Microsoft.CodeAnalysis;
using Organiser.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Organiser.Repositories
{
    public interface ILocationRepository
    {
        List<Department> GetLocations(List<int> locationIds);
    }
}
