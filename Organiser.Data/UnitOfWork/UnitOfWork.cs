using System;
using System.Threading.Tasks;
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
            OrderRepository = new OrderRepository(context);
            UserRoleRepository = new UserRoleRepository(context);
            DepartmentStateRepository = new DepartmentStateRepository(context);
            DepartmentRepository = new DepartmentRepository(context);
            NoteRepository = new NoteRepository(context);
        }

        private AppDbContext _context;

        public LogRepository LogRepository { get; }
        public OrderRepository OrderRepository { get; }
        public UserRepository UserRepository { get; }
        public UserRoleRepository UserRoleRepository { get; }
        public DepartmentStateRepository DepartmentStateRepository { get; }
        public DepartmentRepository DepartmentRepository { get; }
        public NoteRepository NoteRepository { get; }


        public void Dispose()
        {
            _context?.Dispose();
        }
        public void Complete()
        {
            _context.SaveChanges();
        }

        public async Task CompleteAsync()
        {
            await _context.SaveChangesAsync();
        }

        public void Update(object entity) 
        {
            _context.Update(entity);
        }

    }
}
