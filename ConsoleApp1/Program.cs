using CheapestTickets.Server.Services;

namespace CheapestTickets.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Logger.Info("Запуск сервера...");
            string apiToken = "713190d0855f15d4b2a9dd08b6417da9";
            var calculator = new TicketCalculator(apiToken);
            int port = 5000;
            var server = new Networking.Server(port, calculator);
            await server.StartAsync();
        }
    }
}
