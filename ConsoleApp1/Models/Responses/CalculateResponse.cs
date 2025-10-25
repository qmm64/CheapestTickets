using CheapestTickets.Shared.Models;

namespace CheapestTickets.Server.Models.Responses
{
    internal record CalculateResponse(Dictionary<string,decimal>? prices,AppError Error=null);
}
