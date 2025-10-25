using CheapestTickets.Client.Core;
using CheapestTickets.Client.Models;
using CheapestTickets.Client.Services;

namespace CheapestTickets.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await new ClientController().RunAsync();
        }
    }
}
