using CheapestTickets.Server.Database;

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
                string logMessage = $"[{timestamp}] [{source}]{ipPart} {message}";
                Console.WriteLine(logMessage);
                try
                {
                    using var db = new AppDbContext();
                    db.Logs.Add(new LogEntry
                    {
                        Timestamp = DateTime.Now,
                        Source = source,
                        Ip = ip,
                        Message = message
                    });
                    db.SaveChanges();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[LOGGER ERROR] Не удалось записать лог в БД: {ex.Message}");
                }
            }
        }
    }
}
