using CheapestVacationTickets;
using System.Text;
using System.Text.Json;

class Program
{
    static string apiToken = "713190d0855f15d4b2a9dd08b6417da9";
    //static List<((string, string), (int, int))> Flights = new();
    static List<FlightRoute> routes = new List<FlightRoute>();

    static async Task Main(string[] args)
    {
        //Flights.Add((("OVB", "BJS"), (15, 9)));
        //Flights.Add((("BJS", "TYO"), (19, 9)));
        //Flights.Add((("OSA", "SEL"), (27, 9)));
        //Flights.Add((("SEL", "OVB"), (3, 10)));
        routes.Add(new FlightRoute("OVB", "BJS", new DateOnly(2025, 9, 16)));
        routes.Add(new FlightRoute("BJS", "TYO", new DateOnly(2025, 9, 20)));
        routes.Add(new FlightRoute("OSA", "SEL", new DateOnly(2025, 9, 28)));
        routes.Add(new FlightRoute("SEL", "OVB", new DateOnly(2025, 10, 4)));

        try
        {
            Dictionary<string,decimal> prices = new Dictionary<string,decimal>();
            for (int i = 0; i < 5; i++)
            {
                var tasks = routes.Select(x => GetLowestPriceAsync(x.Origin, x.Destination, GetDate(x.Date)));
                var results = await Task.WhenAll(tasks);
                if (results.All(x=>x.isSucces))
                {
                    prices.Add(GetDate(routes[0].Date), results.Sum(x=>x.ticket.price));
                    Console.WriteLine($"Сумарная стоимость вылета {GetDate(routes[0].Date)}: {prices[GetDate(routes[0].Date)]}");
                }
                else
                {
                    Console.WriteLine("По какому-либо перелёту отстутствуют данные о цене");
                }
                routes[0] = routes[0] with { Date =  routes[0].Date.AddDays(1) };
                routes[1] = routes[1] with { Date = routes[1].Date.AddDays(1) };
                routes[2] = routes[2] with { Date = routes[2].Date.AddDays(1) };
                routes[3] = routes[3] with { Date = routes[3].Date.AddDays(1) };

                //Flights[0] = (Flights[0].Item1,PlusDays(Flights[0].Item2));
                //Flights[1] = (Flights[1].Item1, PlusDays(Flights[1].Item2));
                //Flights[2] = (Flights[2].Item1, PlusDays(Flights[2].Item2));
                //Flights[3] = (Flights[3].Item1, PlusDays(Flights[3].Item2));
            }
            decimal min = prices.Values.Min();
            Console.WriteLine($"Дешевле всего вылетать {prices.FirstOrDefault(x => x.Value == min).Key} - {min} руб");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }
    }

    static async Task<FlightSearchResult> GetLowestPriceAsync(string origin, string destination, string departDate)
    {
        using var client = new HttpClient();

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

    static string GetDate(DateOnly date)
    {
        StringBuilder dateString = new StringBuilder();
        dateString.Append($"{date.Year}-");
        if (date.Month < 10)
        {
            dateString.Append('0');
        }
        dateString.Append($"{date.Month}-");
        if (date.Day < 10)
        {
            dateString.Append('0');
        }
        dateString.Append($"{date.Day}");
        return dateString.ToString();
    }

    static string ShowInfoAboutFlight(TicketData ticket)
    {
        return $"Дата: {ticket.departure_at.Substring(0, 10)}, рейс: {ticket.origin}-{ticket.destination}. Цена: {ticket.price}";
    }
}