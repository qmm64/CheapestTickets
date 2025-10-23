namespace CheapestTickets.Server.Models.Responses
{
    internal class FlightResponse
    {
        public Dictionary<string, decimal>? Prices { get; set; }
        public decimal? MinPrice { get; set; }
        public string? MinDate { get; set; }
        public string? Error { get; set; }
    }
}