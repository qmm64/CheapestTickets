
namespace CheapestTickets.Server.Models
{
    internal class FlightRequest
    {
        public List<FlightRoute> Routes { get; set; }
        public int Days { get; set; }
        public bool IncludeAllPrices { get; set; } = false;
    }
}
