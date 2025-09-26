using CheapestTickets.Server.Models;
using CheapestTickets.Server.Models.Responses;
using System.Text.Json;

namespace CheapestTickets.Server.Services
{
    internal class TicketCalculator
    {
        private readonly string apiToken;

        public TicketCalculator(string token)
        {
            apiToken = token;
        }

        public async Task<CalculateResponse> CalculatePricesAsync(List<FlightRoute> routes, int days)
        {
            Dictionary<string, decimal> prices = new Dictionary<string, decimal>();

            for (int i = 0; i < days; i++)
            {
                var shiftedRoutes = routes.Select(r => new FlightRoute(r.Origin, r.Destination, r.Date.AddDays(i))).ToList();
                var calculateResult = await CalculatePriceOfDateDeparture(shiftedRoutes, prices);
                if (!string.IsNullOrEmpty(calculateResult))
                {
                    return new CalculateResponse(null, calculateResult);
                }
            }
            return new CalculateResponse(prices);
        }

        private async Task<string> CalculatePriceOfDateDeparture(List<FlightRoute> routes, Dictionary<string, decimal> prices)
        {
            var tasks = routes.Select(x => GetLowestPriceAsync(x.Origin, x.Destination, GetDate(x.Date)));
            var results = await Task.WhenAll(tasks);
            string dateKey = GetDate(routes[0].Date);

            if (results.All(x => x.IsSucces))
            {
                prices[dateKey] = results.Sum(x => x.Ticket.price);
                Console.WriteLine($"Суммарная стоимость вылета {dateKey}: {prices[dateKey]}");
                return "";
            }
            prices[dateKey] = -1;
            var errors = results.Where(r=>!r.IsSucces && !string.IsNullOrEmpty(r.Error)).Select(r=>r.Error).Distinct();
            if (errors.Any())
            {
                string errorMsg = string.Join("; ", errors);
                return errorMsg;
            }
            return errors.First();
        }

        private async Task<FlightSearchResponse> GetLowestPriceAsync(string origin, string destination, string departDate)
        {
            try
            {
                using (var client = new HttpClient())
                {
                    string url = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={origin}&destination={destination}&departure_at={departDate}&currency=rub&sorting=price&direct=false&limit=1&token={apiToken}";
                    var response = await client.GetAsync(url);
                    string json = await response.Content.ReadAsStringAsync();
                    var data = JsonSerializer.Deserialize<AviasalesResponse>(json);
                    if (data.data == null || data.data.Length == 0)
                    {
                        return new FlightSearchResponse("Не удалось получить данные о рейсе");
                    }
                    return new FlightSearchResponse(data.data[0]);
                }
            }
            catch (HttpRequestException)
            {
                return new FlightSearchResponse("Не удалось подключиться к API");
            }
            catch (TaskCanceledException)
            {
                return new FlightSearchResponse("Время ожидания ответа от API истекло");
            }
            catch (Exception ex) 
            {
                return new FlightSearchResponse(ex.Message);
            }
        }

        private string GetDate(DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}