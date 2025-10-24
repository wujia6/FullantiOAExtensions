using FullantiOAExtensions.Core.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FullantiOAExtensions.Core.Database
{
    public class OaExtendDbContext : DbContext
    {
        public DbSet<Salary> Salaries { get; set; } = default!;
        public DbSet<SalaryManager> SalaryManager { get; set; } = default!;
        public DbSet<Attendance_details> Attendance_Details { get; set; } = default!;
        public DbSet<AutoSettings> AutoSettings { get; set; } = default!;



        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            optionsBuilder.UseSqlServer(config.GetConnectionString("OA"));
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Salary>();
            modelBuilder.Entity<SalaryManager>();
            modelBuilder.Entity<Attendance_details>();
            modelBuilder.Entity<AutoSettings>();
            base.OnModelCreating(modelBuilder);
        }
    }
}
