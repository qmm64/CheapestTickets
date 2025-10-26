using System;

namespace CheapestTickets.Server.Database
{
    public class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.Now;
        public string Source { get; set; } = "SYSTEM";
        public string? ClientId { get; set; }
        public string? ClientIp { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
