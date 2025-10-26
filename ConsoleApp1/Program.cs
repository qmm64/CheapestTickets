using CheapestTickets.Server.Database;
using CheapestTickets.Server.Services;
using Microsoft.EntityFrameworkCore;

namespace CheapestTickets.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var options = new DbContextOptionsBuilder<AppDbContext>().UseNpgsql("Host=localhost;Port=5432;Database=cheapest_tickets;Username=postgres;Password=admin").Options;
            var db = new AppDbContext(options);
            Logger.Init(db);
            RequestLogger.Init(db);
            Logger.Info("Запуск сервера...");
            string apiToken = "713190d0855f15d4b2a9dd08b6417da9";
            var calculator = new TicketCalculator(apiToken);
            int port = 5000;
            var server = new Networking.Server(port, calculator);
            await server.StartAsync();
        }
    }
}
