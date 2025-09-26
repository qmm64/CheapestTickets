using CheapestTickets.Client.Models;
using CheapestTickets.Client.Services;

namespace CheapestTickets.Client
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var routes = new List<FlightRoute>
            {
                new FlightRoute("OVB", "SEL", new DateOnly(2025, 9, 22)),
                new FlightRoute("SEL", "OSA", new DateOnly(2025, 9, 25)),
                new FlightRoute("TYO", "BJS", new DateOnly(2025, 10, 5)),
                new FlightRoute("BJS", "OVB", new DateOnly(2025, 10, 8))
            };

            //var routes = FlightBuilder.AddFlights();

            var countOfDays = RequestHandler.GetCount("Введите количество дней");
            Services.Client client = new();
            var response = await client.SendRequestAsync(new FlightRequest(routes, countOfDays));
            if (response == null)
            {
                Console.WriteLine("Не удалось получить ответ от сервера");
            }
            else if (!String.IsNullOrEmpty(response.Error))
            {
                Console.WriteLine($"Сервер вернул ошибку: {response.Error}");
            }
            else if (response.MinPrice == null || response.MinDate == null)
            {
                Console.WriteLine("Не получилось найти подходящие маршруты");
            }
            else
            {
                Console.WriteLine($"Дешевле всего вылетать {response.MinDate} - {response.MinPrice}");
            }
        }
    }
}
