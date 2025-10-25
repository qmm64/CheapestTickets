using CheapestTickets.Shared.Models;

namespace CheapestTickets.Server.Models
{
    internal class FlightRequest
    {
        public List<Shared.Models.FlightRoute> Routes { get; set; }
        public int Days { get; set; }
        public bool IncludeAllPrices { get; set; } = false;
    }
}
