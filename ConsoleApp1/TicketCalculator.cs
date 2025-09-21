using CheapestVacationTickets;
using System.Text.Json;

namespace CheapestTickets.Server
{
    internal class TicketCalculator
    {
        private readonly string apiToken;

        public TicketCalculator(string token)
        {
            apiToken = token;
        }

        public async Task<Dictionary<string, decimal>> CalculatePricesAsync(List<FlightRoute> routes, int days)
        {
            Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

            for (int i = 0; i < days; i++)
            {
                await CalculatePriceOfDateDeparture(routes, prices);
                AddDayToTheDate(routes);
            }

            return prices;
        }

        private async Task CalculatePriceOfDateDeparture(List<FlightRoute> routes, Dictionary<string, decimal> prices)
        {
            var tasks = routes.Select(x => GetLowestPriceAsync(x.Origin, x.Destination, GetDate(x.Date)));
            var results = await Task.WhenAll(tasks);

            if (results.All(x => x.IsSucces))
            {
                prices.Add(GetDate(routes[0].Date), results.Sum(x => x.Ticket.price));
                Console.WriteLine($"Суммарная стоимость вылета {GetDate(routes[0].Date)}: {prices[GetDate(routes[0].Date)]}");
            }
            else
            {
                prices.Add(GetDate(routes[0].Date), -1);
            }
        }

        private async Task<FlightSearchResult> GetLowestPriceAsync(string origin, string destination, string departDate)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={origin}&destination={destination}&departure_at={departDate}&currency=rub&sorting=price&direct=false&limit=1&token={apiToken}";
                    var response = await client.GetAsync(url);
                    response.EnsureSuccessStatusCode();

                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<AviasalesResponse>(json);

                    if (data.data == null || data.data.Length == 0)
                        return new FlightSearchResult(false);

                    return new FlightSearchResult(true, data.data[0]);
                }
            }
            catch
            {
                return new FlightSearchResult(false);
            }
        }

        private string GetDate(DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }

        private void AddDayToTheDate(List<FlightRoute> routes)
        {
            foreach (var flight in routes)
            {
                flight.Date = flight.Date.AddDays(1);
            }
        }
    }
}