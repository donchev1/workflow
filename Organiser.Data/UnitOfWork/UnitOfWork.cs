using System;
using Organiser.Data.Context;
using Organiser.Data.Repositories;

namespace Organiser.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            LogRepository = new LogRepository(context);
            UserRepository = new UserRepository(context);
        }

        private AppDbContext _context;

        public LogRepository LogRepository { get; }
        public IUserRepository UserRepository { get; }

        public void Dispose()
        {
            _context?.Dispose();
        }
    }
}
