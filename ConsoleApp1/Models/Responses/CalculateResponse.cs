using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CheapestTickets.Shared.Models;

namespace CheapestTickets.Server.Models.Responses
{
    internal record CalculateResponse(Dictionary<string,decimal>? prices,AppError Error=null);
}
