using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using RentMyCPU.Backend.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RentMyCPU.Backend.Data
{
    public class ApplicationDbContext : IdentityDbContext<User, IdentityRole<Guid>, Guid>
    {
        public DbSet<RequestorTask> RequestorTasks { get; set; }
        public DbSet<WorkerTask> WorkerTasks { get; set; }
        public DbSet<Purchase> Purchases { get; set; }
        public DbSet<Worker> Workers { get; set; } 
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        public static ApplicationDbContext FromConnectionString(string connectionString)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlServer(connectionString, dbContextOptions => dbContextOptions.EnableRetryOnFailure());
            return new ApplicationDbContext(optionsBuilder.Options);
        }
        public static ApplicationDbContext FromConnectionString(IConfiguration config)
        {
            return FromConnectionString(config.GetConnectionString("DefaultConnection"));
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.Entity<RequestorTask>().HasOne(e => e.Requestor).WithMany(e => e.RequestorTasks).HasForeignKey(e => e.RequestorId);

            builder.Entity<WorkerTask>().HasOne(e => e.RequestorTask).WithMany(e => e.WorkerTasks).HasForeignKey(e => e.RequestorTaskId).OnDelete(DeleteBehavior.Restrict);
            builder.Entity<WorkerTask>().HasOne(e => e.Provider).WithMany(e => e.WorkerTasks).HasForeignKey(e => e.ProviderId).OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Purchase>().HasOne(e => e.User).WithMany(e => e.Purchases).HasForeignKey(e => e.UserId);

            builder.Entity<Worker>().HasOne(e => e.User).WithMany(e => e.Workers).HasForeignKey(e => e.UserId);
            builder.Entity<Worker>().HasMany(e => e.Tasks).WithOne(e => e.Worker).HasForeignKey(e => e.WorkerId);

            builder.Entity<User>(eb =>
            {
                eb.Property(b => b.Credits).HasColumnType("decimal(18, 6)");
            });
            base.OnModelCreating(builder);
        }
    }
}
