namespace CheapestTickets.Server.Models
{
    internal class TicketData
    {
        public decimal price { get; set; }
        public string airline { get; set; }
        public string flight_number { get; set; }
        public string departure_at { get; set; }
        public string return_at { get; set; }
        public int transfers { get; set; }
        public int duration { get; set; }
        public string origin { get; set; }
        public string destination { get; set; }
    }
}
