using CheapestTickets.Server.Models;

namespace CheapestTickets.Server.Services
{
    internal static class FlightBuilder
    {
        public static FlightRoute CreateFlight(string origin, string destination, DateOnly date)
        {
            return new FlightRoute
            {
                Origin = origin,
                Destination = destination,
                Date = date
            };
        }

        public static void AddFlights(List<FlightRoute> routes, IEnumerable<(string origin, string destination, DateOnly date)> flights)
        {
            foreach (var flight in flights)
            {
                routes.Add(CreateFlight(flight.origin, flight.destination, flight.date));
            }
        }
    }
}