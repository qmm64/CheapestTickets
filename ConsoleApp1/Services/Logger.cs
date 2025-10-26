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

        public static void Info(string message, Sources source = Sources.SYSTEM, ClientContext? client = null)
        {
            lock (_lock)
            {
                string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                string ip = client != null ? $" [{client.Ip}]-" : "";
                string id = client != null ? $"[{client.Id.ToString()}]" : "";
                Console.WriteLine($"[{timestamp}] [{source}]{ip}{id} {message}");
            }

            try
            {
                if (_db != null)
                {
                    _db.Logs.Add(new LogEntry
                    {
                        Timestamp = DateTime.UtcNow,
                        Source = source.ToString(),
                        ClientId = client != null ? client.Id.ToString() : "",
                        ClientIp = client != null ? client.Ip : "",
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

        public enum Sources
        {
            SYSTEM,
            CLIENT
        }
    }
}
