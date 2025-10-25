using CheapestTickets.Shared.Models;

namespace CheapestTickets.Client.Core
{
    internal class ClientController
    {
        public async Task RunAsync()
        {
            var ui = new UserInterface();

            var routes = new List<FlightRoute>
            {
                new FlightRoute("OVB", "SEL", new DateOnly(2025, 10, 25)),
                new FlightRoute("SEL", "OSA", new DateOnly(2025, 10, 28)),
                new FlightRoute("TYO", "BJS", new DateOnly(2025, 11, 7)),
                new FlightRoute("BJS", "OVB", new DateOnly(2025, 11, 10))
            };

            while (true) 
            {
                //var routes = FlightBuilder.AddFlights();
                int days = RequestHandler.GetCount("Введите количество дней");
                var client = new Services.Client();
                var request = new FlightRequest(routes, days);
                var response = await client.SendRequestAsync(request);
                ui.DisplayResponse(response);
            }
        }
    }
}
