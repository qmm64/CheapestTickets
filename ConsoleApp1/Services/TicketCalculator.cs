using CheapestTickets.Server.Models;
using CheapestTickets.Server.Models.Responses;
using System.Text.Json;

namespace CheapestTickets.Server.Services
{
    internal class TicketCalculator
    {
        private readonly string apiToken;
        private readonly HttpClient _httpClient;

        public TicketCalculator(string token)
        {
            apiToken = token;
            _httpClient = new HttpClient();
        }

        public async Task<CalculateResponse> CalculatePricesAsync(List<FlightRoute> routes, int days)
        {
            Dictionary<string, decimal> prices = new();

            for (int i = 0; i < days; i++)
            {
                var shiftedRoutes = routes.Select(r => new FlightRoute(r.Origin, r.Destination, r.Date.AddDays(i))).ToList();
                var error = await CalculateDayAsync(shiftedRoutes, prices);
                if (error.Type != ErrorType.None && error.Type != ErrorType.NoData)
                {
                    return new CalculateResponse(null, error.Message);
                }
            }

            return new CalculateResponse(prices);
        }

        private async Task<AppError> CalculateDayAsync(List<FlightRoute> routes, Dictionary<string, decimal> prices)
        {
            var tasks = routes.Select(r =>
                GetLowestPriceAsync(r.Origin, r.Destination, GetDate(r.Date))
            );

            var results = await Task.WhenAll(tasks);
            string dateKey = GetDate(routes[0].Date);
            var critical = results.FirstOrDefault(r =>!r.IsSuccess && r.Error.Type is ErrorType.ApiUnavailable or ErrorType.Timeout or ErrorType.Internal);
            if (critical != null)
            {
                Console.WriteLine($"Ошибка при обработке {dateKey}: {critical.Error.Message}");
                return critical.Error;
            }
            if (results.Any(r => !r.IsSuccess && r.Error.Type == ErrorType.NoData))
            {
                Console.WriteLine($"Нет данных для даты {dateKey}");
                prices[dateKey] = -1;
                return AppError.NoData($"Нет данных для даты {dateKey}");
            }
            decimal totalPrice = results.Sum(r => r.Ticket!.price);
            prices[dateKey] = totalPrice;
            Console.WriteLine($"Суммарная стоимость вылета {dateKey}: {totalPrice}");
            return AppError.None;
        }

        private async Task<FlightSearchResponse> GetLowestPriceAsync(string origin, string destination, string departDate)
        {
            try
            {
                string url =$"https://api.travelpayouts.com/aviasales/v3/prices_for_dates?origin={origin}&destination={destination}&departure_at={departDate}&currency=rub&sorting=price&direct=false&limit=1&token={apiToken}";
                var response = await _httpClient.GetAsync(url);
                if (!response.IsSuccessStatusCode)
                {
                    return new FlightSearchResponse(AppError.ApiUnavailable($"API вернул статус {response.StatusCode}"));
                }
                string json = await response.Content.ReadAsStringAsync();
                var data = JsonSerializer.Deserialize<AviasalesResponse>(json);
                if (data?.data == null || data.data.Length == 0)
                {
                    return new FlightSearchResponse(
                        AppError.NoData($"Нет рейсов {origin}-{destination} на {departDate}")
                    );
                }
                return new FlightSearchResponse(data.data[0]);
            }
            catch (HttpRequestException)
            {
                return new FlightSearchResponse(AppError.ApiUnavailable("Ошибка сети или API недоступен"));
            }
            catch (TaskCanceledException)
            {
                return new FlightSearchResponse(AppError.Timeout("Таймаут ожидания API"));
            }
            catch (Exception ex)
            {
                return new FlightSearchResponse(AppError.Internal(ex.Message));
            }
        }

        private string GetDate(DateOnly date)
        {
            return date.ToString("yyyy-MM-dd");
        }
    }
}
