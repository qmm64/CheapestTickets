
namespace CheapestVacationTickets
{
    internal class AviasalesResponse
    {
        public bool Success { get; set; }
        public TicketData[] Data { get; set; }
        public string Currency { get; set; }
    }
}
