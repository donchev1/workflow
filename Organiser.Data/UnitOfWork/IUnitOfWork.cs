using System;

namespace Organiser.Data.UnitOfWork
{
    interface IUnitOfWork : IDisposable
    {
        ILocationRepository LocalRepository { get; }
    }
}
