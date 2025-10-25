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

        public async Task<CalculateResponse> CalculatePricesAsync(List<FlightRoute> routes, int days, CancellationToken token)
        {
            Dictionary<string, decimal> prices = new();
            for (int i = 0; i < days; i++)
            {
                token.ThrowIfCancellationRequested();
                var shiftedRoutes = routes.Select(r => new FlightRoute(r.Origin, r.Destination, r.Date.AddDays(i))).ToList();
                var result = await CalculatePriceOfDateDeparture(shiftedRoutes, prices, token);
                if (result != null)
                {
                    //return new CalculateResponse(null, result);
                    Console.WriteLine(result);
                }
            }
            return new CalculateResponse(prices);
        }

        private async Task<AppError?> CalculatePriceOfDateDeparture(List<FlightRoute> routes,Dictionary<string, decimal> prices,CancellationToken token)
        {
            token.ThrowIfCancellationRequested();
            var tasks = routes.Select(r => GetLowestPriceAsync(r.Origin, r.Destination, GetDate(r.Date), token));
            var results = await Task.WhenAll(tasks);
            string dateKey = GetDate(routes[0].Date);
            if (results.All(r => r.IsSuccess))
            {
                prices[dateKey] = results.Sum(r => r.Ticket!.price);
                Console.WriteLine($"Суммарная стоимость вылета {dateKey}: {prices[dateKey]}");
                return null;
            }
            prices[dateKey] = -1;
            var errors = results.Where(r => !r.IsSuccess && r.Error.Type != ErrorType.None).Select(r => r.Error).Distinct().ToList();
            if (errors.Any())
            {
                return errors.First();
            }
            return AppError.Internal("Неизвестная ошибка при расчёте стоимости");
        }

        private async Task<FlightSearchResponse> GetLowestPriceAsync(string origin, string destination, string departDate, CancellationToken token)
        {
            try
            {
                using var client = new HttpClient();
                token.ThrowIfCancellationRequested();
                string url = $"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={origin}&destination={destination}&departure_at={departDate}&currency=rub&sorting=price&direct=false&limit=1&token={apiToken}";
                var response = await client.GetAsync(url, token);
                string json = await response.Content.ReadAsStringAsync(token);
                var data = JsonSerializer.Deserialize<AviasalesResponse>(json);
                if (data?.data == null || data.data.Length == 0)
                {
                    return new FlightSearchResponse(AppError.NoData("Не удалось получить данные о рейсе"));
                }
                return new FlightSearchResponse(data.data[0]);
            }
            catch (OperationCanceledException)
            {
                return new FlightSearchResponse(AppError.Internal("Расчёт отменён пользователем"));
            }
            catch (HttpRequestException)
            {
                return new FlightSearchResponse(AppError.ApiUnavailable("Не удалось подключиться к API"));
            }
            catch (Exception ex)
            {
                return new FlightSearchResponse(AppError.Internal(ex.Message));
            }
        }

        private string GetDate(DateOnly date) => date.ToString("yyyy-MM-dd");
    }
}
