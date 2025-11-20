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
                new FlightRoute("OVB", "SEL", DateOnly.FromDateTime(DateTime.Now)),
                new FlightRoute("SEL", "OSA", DateOnly.FromDateTime(DateTime.Now.AddDays(3))),
                new FlightRoute("TYO", "BJS", DateOnly.FromDateTime(DateTime.Now.AddDays(13))),
                new FlightRoute("BJS", "OVB", DateOnly.FromDateTime(DateTime.Now.AddDays(16)))
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
