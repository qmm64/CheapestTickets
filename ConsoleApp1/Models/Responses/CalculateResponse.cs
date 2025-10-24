using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CheapestTickets.Server.Models.Responses
{
    internal record CalculateResponse(Dictionary<string,decimal>? prices,AppError Error=null);
}
