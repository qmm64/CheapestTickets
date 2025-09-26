namespace CheapestTickets.Server.Models.Responses
{
    internal class FlightSearchResponse
    {
        public bool IsSucces { get; }
        public string? Error { get; }
        public TicketData? Ticket { get; }

        public FlightSearchResponse(TicketData ticketData)
        {
            IsSucces = true;
            Ticket = ticketData;
        }
        public FlightSearchResponse(string error) 
        {
            IsSucces = false;
            Error = error;
        }
    }
}
