using CheapestVacationTickets.Server.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CheapestVacationTickets.Server
{
    class Program
    {
        static async Task Main(string[] args)
        {
            string apiToken = "713190d0855f15d4b2a9dd08b6417da9";
            var calculator = new TicketCalculator(apiToken);

            var routes = new List<FlightRoute>
            {
                new FlightRoute("OVB", "SEL", new DateOnly(2025, 9, 21)),
                new FlightRoute("SEL", "OSA", new DateOnly(2025, 9, 24)),
                new FlightRoute("TYO", "BJS", new DateOnly(2025, 10, 4)),
                new FlightRoute("BJS", "OVB", new DateOnly(2025, 10, 7))
            };

            try
            {
                Dictionary<string, decimal> prices = await calculator.CalculatePricesAsync(routes,5);
                var validPrices = prices.Where(p => p.Value >= 0).ToList();
                if (validPrices.Count > 0)
                {
                    var minPrice = validPrices.Min(p => p.Value);
                    var minDate = validPrices.First(p => p.Value == minPrice).Key;
                    Console.WriteLine($"Дешевле всего вылетать {minDate} - {minPrice} руб");
                }
                else
                {
                    Console.WriteLine("Нет доступных данных о ценах");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при расчёте: {ex.Message}");
            }
        }
    }
}