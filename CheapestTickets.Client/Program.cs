using CheapestTickets.Client.Core;

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
