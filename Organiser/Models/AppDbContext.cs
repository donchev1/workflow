using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace Organiser.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Order> Orders { get; set; }
        public DbSet<LocState> LocStates { get; set; }
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

            modelBuilder.Entity<LocState>()
            .HasOne(ls => ls.Order)
            .WithMany(ls => ls.LocStates)
            .HasForeignKey(ls => ls.OrderId)
            .HasConstraintName("ForeignKey_LocState_Order");
        }
      
    }
}
