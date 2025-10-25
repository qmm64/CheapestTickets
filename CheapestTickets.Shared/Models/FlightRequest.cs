
namespace CheapestTickets.Shared.Models
{
    public class FlightRequest
    {
        public List<FlightRoute> Routes { get; set; }
        public int Days { get; set; }
        public bool IncludeAllPrices { get; set; } = false;

        public FlightRequest(List<FlightRoute> routes, int days, bool includeAllPrices = false)
        {
            Routes = routes;
            Days = days;
            IncludeAllPrices = includeAllPrices;
        }
    }
}
