using ETA.Integrator.Server.Entities;
using Microsoft.EntityFrameworkCore;

namespace ETA.Integrator.Server.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Test> Tests { get; set; } = null!;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {


            string projectRoot = FindProjectRoot();
            string databasePath = Path.Combine(projectRoot, "Data", "ETA_DB.db");

            optionsBuilder.UseSqlite($"Data Source={databasePath}");

        }

        private static string FindProjectRoot()
        {
            // Start from the current base directory
            string currentDirectory = AppDomain.CurrentDomain.BaseDirectory;

            // Traverse up until we find the .csproj file
            DirectoryInfo? directory = new DirectoryInfo(currentDirectory);
            while (directory != null)
            {
                if (directory.GetFiles("*.csproj").Length > 0)
                {
                    return directory.FullName; // Found the project root
                }
                directory = directory.Parent; // Move up one level
            }

            // If no .csproj file is found, return null
            return null;
        }
    }
}
