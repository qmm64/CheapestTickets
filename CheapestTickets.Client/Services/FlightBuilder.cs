using CheapestTickets.Shared.Models;

namespace CheapestTickets.Client.Services
{
    internal static class FlightBuilder
    {
        public static List<FlightRoute> AddFlights() 
        { 
            List<FlightRoute> routes = new List<FlightRoute>();
            int countOfFlights = RequestHandler.GetCount("Введите количество перелётов"); 
            for (int i = 0; i < countOfFlights; i++) 
            { 
                routes.Add(CreateFlight()); 
            } 
            return routes;
        }
        public static FlightRoute CreateFlight() 
        { 
            FlightRoute flight = new FlightRoute(); 
            flight.Origin = RequestHandler.GetAirportCode("Введите аэропорт вылета"); 
            flight.Destination = RequestHandler.GetAirportCode("Введите аэропорт прилёта"); 
            flight.Date = RequestHandler.GetDate("Введите дату вылета в формате yyyy-mm-dd"); 
            return flight; 
        }
    }
}