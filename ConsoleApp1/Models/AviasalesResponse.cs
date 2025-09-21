
namespace CheapestTickets.Server.Models
{
    internal class AviasalesResponse
    {
        public bool success { get; set; }
        public TicketData[] data { get; set; }
        public string currency { get; set; }
    }
}
