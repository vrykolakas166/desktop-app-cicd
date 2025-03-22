using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using NETX.Common;
using NETX.Core.Models;

namespace NETX.Core
{
    public class NXDbContext : DbContext
    {
        public DbSet<Setting> Settings { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var directory = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), Constants.APPLICATION_NAME);

                try
                {
                    if (!Directory.Exists(directory))
                    {
                        Directory.CreateDirectory(directory);
                    }

                    var connectionString = new SqliteConnectionStringBuilder()
                    {
                        DataSource = Path.Combine(directory, $"main.{Constants.APPLICATION_NAME.ToLower()}")
                    };

                    optionsBuilder.UseSqlite(connectionString.ToString());
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error configuring DbContext: {ex.Message}");
                }
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Setting>(entity =>
            {
                // Configure the 'Key' column
                entity.Property(s => s.Key)
                .IsRequired()
                .HasMaxLength(255);

                // Configure the 'Value' column
                entity.Property(s => s.Value)
                    .HasMaxLength(500);

                // Ensure 'Key' is unique
                entity.HasIndex(s => s.Key)
                    .IsUnique();

            });
        }
    }
}
