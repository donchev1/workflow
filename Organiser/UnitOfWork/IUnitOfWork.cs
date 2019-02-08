using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Organiser.Data;

namespace Organiser.UnitOfWork
{
    interface IUnitOfWork : IDisposable
    {
        ILocationRepository LocationRepository { get; }

        int Complete();
    }
}
