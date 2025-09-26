
namespace CheapestTickets.Server.Models
{
    internal class FlightRoute
    {
        public string Origin { get; set; }
        public string Destination { get; set; }
        public DateOnly Date { get; set; }
        public FlightRoute(string Origin,string Destination,DateOnly Date)
        {
            this.Origin = Origin;
            this.Destination = Destination;
            this.Date = Date;
        }
    }
}
