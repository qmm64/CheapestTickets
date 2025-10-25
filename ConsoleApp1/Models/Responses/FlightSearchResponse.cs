using CheapestTickets.Server.Models;
using CheapestTickets.Server.Models.Responses;
using CheapestTickets.Shared.Models;

internal class FlightSearchResponse
{
    public bool IsSuccess { get; }
    public TicketData? Ticket { get; }
    public AppError Error { get; }

    public FlightSearchResponse(TicketData ticket)
    {
        IsSuccess = true;
        Ticket = ticket;
        Error = AppError.None();
    }

    public FlightSearchResponse(AppError error)
    {
        IsSuccess = false;
        Error = error;
    }
}
