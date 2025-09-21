using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestTickets.Server.Models
{
    internal class FlightRequest
    {
        public List<FlightRoute> Routes { get; set; }
        public int Days { get; set; }
        public bool IncludeAllPrices { get; set; } = false;
    }
}
