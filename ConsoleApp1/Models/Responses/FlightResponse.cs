using CheapestTickets.Server.Models.Responses;
using System.Collections.Generic;

namespace CheapestTickets.Server.Models
{
    internal class FlightResponse
    {
        public decimal MinPrice { get; set; }
        public string? MinDate { get; set; }
        public Dictionary<string, decimal>? Prices { get; set; }
        public AppError? Error { get; set; }
    }
}
