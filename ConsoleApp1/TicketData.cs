
namespace CheapestVacationTickets
{
    internal class TicketData
    {
        public decimal Price { get; set; }
        public string Airline { get; set; }
        public string Flight_number { get; set; }
        public string Departure_at { get; set; }
        public string Return_at { get; set; }
        public int Transfers { get; set; }
        public int Duration { get; set; }
        public string Origin { get; set; }
        public string Destination { get; set; }
    }
}
