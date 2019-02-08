using System;
using System.Collections.Generic;
using System.Text;
using Organiser.Data.Repositories;
using Organiser.Data.Models;

namespace Organiser.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        ILogRepository LogRepository { get; }
        IUserRepository UserRepository { get; }

        void Dispose();
    }
}
