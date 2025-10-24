using System;

namespace CheapestTickets.Server.Services
{
    internal static class Logger
    {
        private static readonly object _lock = new();

        public static void Info(string message, string source = "")
        {
            Log("INFO", message, ConsoleColor.Gray, source);
        }

        public static void Warning(string message, string source = "")
        {
            Log("WARN", message, ConsoleColor.Yellow, source);
        }

        public static void Error(string message, string source = "")
        {
            Log("ERROR", message, ConsoleColor.Red, source);
        }

        private static void Log(string level, string message, ConsoleColor color, string source)
        {
            lock (_lock)
            {
                var timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
                var prefix = string.IsNullOrEmpty(source)
                    ? $"[{timestamp}] [{level}]"
                    : $"[{timestamp}] [{source}] [{level}]";

                Console.ForegroundColor = color;
                Console.WriteLine($"{prefix} {message}");
                Console.ResetColor();
            }
        }
    }
}
