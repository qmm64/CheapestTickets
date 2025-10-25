
using CheapestTickets.Server.Models.Responses;

namespace CheapestTickets.Client.Models
{
    internal class FlightResponse
    {
        public Dictionary<string, decimal>? Prices { get; set; }
        public decimal? MinPrice { get; set; }
        public string? MinDate { get; set; }
        public AppError? Error { get; set; }
    }
}