using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Organiser.Data.Repositories;
using Organiser.Data.Models;

namespace Organiser.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        LogRepository LogRepository { get; }
        UserRepository UserRepository { get; }
        OrderRepository OrderRepository { get; }
        DepartmentStateRepository DepartmentStateRepository { get; }
        UserRoleRepository UserRoleRepository { get; }

        void Complete();
        Task CompleteAsync();

        void Update(object entity);


    }
}
