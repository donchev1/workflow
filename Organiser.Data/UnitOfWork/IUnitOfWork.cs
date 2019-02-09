using System;
using System.Collections.Generic;
using System.Text;
using Organiser.Data.Repositories;
using Organiser.Data.Models;

namespace Organiser.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        LogRepository LogRepository { get; }
        UserRepository UserRepository { get; }

    }
}
