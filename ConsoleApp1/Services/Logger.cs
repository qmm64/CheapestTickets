using CheapestTickets.Server.Database;

namespace CheapestTickets.Server.Services
{
    internal static class Logger
    {
        private static readonly object _lock = new();
        private static AppDbContext? _db;

        public static void Init(AppDbContext db)
        {
            _db = db;
        }

        public static void Info(string message, string source = "SYSTEM", string? ip = null)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string ipPart = ip != null ? $" [{ip}]" : "";
                Console.WriteLine($"[{timestamp}] [{source}]{ipPart} {message}");
            }

            try
            {
                if (_db != null)
                {
                    _db.Logs.Add(new LogEntry
                    {
                        Timestamp = DateTime.UtcNow,
                        Source = source,
                        ClientIp = ip,
                        Message = message
                    });
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[LOGGER ERROR] Не удалось записать лог в БД: {ex.GetBaseException().Message}");
            }
        }
    }
}
