using ETA.Integrator.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETA.Integrator.Server.Data
{
    public class AppDbContext : DbContext
    {

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
        }
        public DbSet<Test> Tests { get; set; } = null!;
        public DbSet<SettingsStep> SettingsSteps { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<SettingsStep>().HasData(
                new SettingsStep
                {
                    Order = 1,
                    Name = "connection-settings",
                    Data = null
                },
                new SettingsStep
                {
                    Order = 2,
                    Name = "issuer-settings",
                    Data = null
                });
        }
    }
}
