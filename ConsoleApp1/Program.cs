using CheapestVacationTickets;
using System.Text.Json;

class Program
{
    static string apiToken = "713190d0855f15d4b2a9dd08b6417da9";
    static List<FlightRoute> routes = new List<FlightRoute>();

    static async Task Main(string[] args)
    {
        routes.Add(new FlightRoute("OVB", "SEL", new DateOnly(2025, 9, 19)));
        routes.Add(new FlightRoute("SEL", "OSA", new DateOnly(2025, 9, 21)));
        routes.Add(new FlightRoute("TYO", "BJS", new DateOnly(2025, 10, 2)));
        routes.Add(new FlightRoute("BJS", "OVB", new DateOnly(2025, 10, 5)));
        //AddFlights();

        try
        {
            Dictionary<string,decimal> prices = new Dictionary<string,decimal>();
            for (int i = 0; i < 90; i++)
            {
                await CalculatePriceOfDateDeparture(prices);
            }
            decimal min = prices.Values.Min();
            Console.WriteLine($"Дешевле всего вылетать {prices.FirstOrDefault(x => x.Value == min).Key} - {min} руб");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    private static async Task CalculatePriceOfDateDeparture(Dictionary<string, decimal> prices)
    {
        try
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
                Console.WriteLine("По какому-либо перелёту отсутствуют данные о цене");
            }
            AddDayToTheDate();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static async Task<FlightSearchResult> GetLowestPriceAsync(string origin, string destination, string departDate)
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
                {
                    return new FlightSearchResult(false);
                }
                return new FlightSearchResult(true, data.data[0]);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
            return new FlightSearchResult(false);
        }
    }

    static string GetDate(DateOnly date)
    {
        return date.ToString("yyyy-MM-dd");
    }

    //static string ShowInfoAboutFlight(TicketData ticket)
    //{
    //    return $"Дата: {ticket.departure_at.Substring(0, 10)}, рейс: {ticket.origin}-{ticket.destination}. Цена: {ticket.price}";
    //}

    static void AddFlights()
    {
        Console.WriteLine("Введите количество перелётов");
        int countOfFlights = int.Parse(Console.ReadLine());
        for (int i = 0; i < countOfFlights; i++)
        {
            routes.Add(CreateFlight());
        }
    }

    static FlightRoute CreateFlight()
    {
        FlightRoute flight = new FlightRoute();
        Console.WriteLine("Введите аэропорт вылета");
        flight.Origin = Console.ReadLine();
        Console.WriteLine("Введите аэропорт прилёта");
        flight.Destination = Console.ReadLine();
        Console.WriteLine("Введите дату вылета в формате yyyy-mm-dd");
        flight.Date = DateOnly.Parse(Console.ReadLine());
        return flight;
    }

    static void AddDayToTheDate()
    {
        foreach (var flight in routes)
        {
            flight.Date = flight.Date.AddDays(1);
        }
    }
}