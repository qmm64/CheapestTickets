using System.ComponentModel.DataAnnotations;

namespace CheapestTickets.Server.Models.Database
{
    public class UserRequestLog
    {
        [Key]
        public int Id { get; set; }
        public Guid ClientId { get; set; }
        public string? IpAddress { get; set; }
        public DateTime RequestTime { get; set; } = DateTime.UtcNow;
        public int Days { get; set; }
        public decimal? MinPrice { get; set; }
        public string? MinDate { get; set; }
        public string? Error { get; set; }
    }
}
