using Microsoft.EntityFrameworkCore;

namespace CheapestTickets.Server.Database
{
    internal class AppDbContext : DbContext
    {
        public DbSet<LogEntry> Logs => Set<LogEntry>();

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            const string connectionString = "Host=localhost;Port=5432;Database=cheapest_tickets;Username=agmin;Password=admin";
            optionsBuilder.UseNpgsql(connectionString);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<LogEntry>(entity =>
            {
                entity.ToTable("logs");
                entity.HasKey(e => e.Id);
                entity.Property(e => e.Timestamp).IsRequired();
                entity.Property(e => e.Source).HasMaxLength(100);
                entity.Property(e => e.Ip).HasMaxLength(45);
                entity.Property(e => e.Message).HasMaxLength(2000);
            });
        }
    }
}
