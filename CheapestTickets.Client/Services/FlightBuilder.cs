using CheapestTickets.Server.Models;
using CheapestVacationTickets;

namespace CheapestTickets.Server.Services
{
    internal static class FlightBuilder
    {
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