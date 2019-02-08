using Microsoft.EntityFrameworkCore;

namespace Organiser.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<DepartmentState> DepartmentStates { get; set; }
        public DbSet<Department> Departments { get; set; }

        public DbSet<UserRole> UserRoles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Note> Notes { get; set; }
        public DbSet<NewMessagesMonitor> NewMessagesMonitor { get; set; }
        public DbSet<Log> Logs { get; set; }



        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<UserRole>()
           .HasOne(ur => ur.User)
           .WithMany(u => u.UserRoles)
           .HasForeignKey(ur => ur.UserId)
           .HasConstraintName("ForeignKey_UserRole_User");

            modelBuilder.Entity<DepartmentState>()
            .HasOne(ls => ls.Order)
            .WithMany(ls => ls.DepartmentStates)
            .HasForeignKey(ls => ls.OrderId)
            .HasConstraintName("ForeignKey_DepartmentState_Order");
        }
      
    }
}
