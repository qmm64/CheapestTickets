
namespace CheapestTickets.Server.Models
{
    internal class FlightSearchResult
    {
        public bool IsSucces { get; }
        public string? Error { get; }
        public TicketData? Ticket { get; }

        public FlightSearchResult(TicketData ticketData)
        {
            IsSucces = true;
            Ticket = ticketData;
        }
        public FlightSearchResult(string error) 
        {
            IsSucces = false;
            Error = error;
        }
    }
}
