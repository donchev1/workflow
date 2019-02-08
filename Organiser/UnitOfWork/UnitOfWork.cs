using System;
using SQLitePCL;

namespace Organiser.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly Context _context;

        public ILocationRepository LocationRepository { get; }

        public UnitOfWork(Context context)
        {
            _context = context;

            LocationRepository = new ILocationRepository(context);
        }

        public void Dispose()
        {
            _context.Dispose();
        }

        public int Complete()
        {
            _context.SubmitChanges();
        }
    }
}
