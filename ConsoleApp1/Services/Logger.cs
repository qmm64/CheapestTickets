
namespace CheapestTickets.Server.Services
{
    internal static class Logger
    {
        private static readonly object _lock = new();

        public static void Info(string message, string source = "SYSTEM", string? ip = null)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string ipPart = ip != null ? $" [{ip}]" : "";
                Console.WriteLine($"[{timestamp}] [{source}]{ipPart} {message}");
            }
        }
    }
}
