namespace CheapestTickets.Server.Services
{
    internal static class Logger
    {
        private static readonly object _lock = new();

        public static void Info(string message, string clientInfo = "")
        {
            Log("INFO", message, ConsoleColor.Gray, clientInfo);
        }

        public static void Warning(string message, string clientInfo = "")
        {
            Log("WARN", message, ConsoleColor.Yellow, clientInfo);
        }

        public static void Error(string message, string clientInfo = "")
        {
            Log("ERROR", message, ConsoleColor.Red, clientInfo);
        }

        private static void Log(string level, string message, ConsoleColor color, string clientInfo)
        {
            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var prefix = string.IsNullOrEmpty(clientInfo)
                    ? $"[{timestamp}] [{level}]"
                    : $"[{timestamp}] [{level}] [{clientInfo}]";

                Console.ForegroundColor = color;
                Console.WriteLine($"{prefix} {message}");
                Console.ResetColor();
            }
        }
    }
}
