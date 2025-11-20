using CheapestTickets.Server.Models.Database;
using Microsoft.EntityFrameworkCore;

namespace CheapestTickets.Server.Database
{
    public class AppDbContext : DbContext
    {
        public DbSet<LogEntry> Logs { get; set; }
        public DbSet<UserRequestLog> UserRequests { get; set; }

        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {
            Database.EnsureCreated();
        }
    }
}
