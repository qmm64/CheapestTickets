
namespace CheapestTickets.Server.Database
{
    internal class LogEntry
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string Source { get; set; } = string.Empty;
        public string? Ip { get; set; }
        public string Message { get; set; } = string.Empty;
    }
}
