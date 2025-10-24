using FullantiOAExtensions.Core.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace FullantiOAExtensions.Core.Database
{
    public class EFCoreDbContext: DbContext
    {
        public DbSet<MFPOS> MFPOS { get; set; } = default!;
        public DbSet<TFPOS> TFPOS { get; set; } = default!;
        public DbSet<MFTY> MFTY { get; set; } = default!;
        public DbSet<TFTY> TFTY { get; set; } = default!;

        public EFCoreDbContext(DbContextOptions<EFCoreDbContext> options) : base(options) { }

        //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        //{
        //    var config = new ConfigurationBuilder()
        //        .SetBasePath(Directory.GetCurrentDirectory())
        //        .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
        //        .Build();

        //    optionsBuilder.UseSqlServer(config.GetConnectionString("MSSQL"));
        //    base.OnConfiguring(optionsBuilder);
        //}

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<MFPOS>().HasNoKey();
            modelBuilder.Entity<TFPOS>().HasNoKey();
            modelBuilder.Entity<MFTY>().HasNoKey();
            modelBuilder.Entity<TFTY>().HasNoKey();
            base.OnModelCreating(modelBuilder);
        }
    }
}
