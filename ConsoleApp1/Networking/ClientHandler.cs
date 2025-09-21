using CheapestTickets.Server.Models;
using CheapestTickets.Server.Services;
using CheapestVacationTickets.Server.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CheapestVacationTickets.Server.Networking
{
    internal class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly TicketCalculator _calculator;

        public ClientHandler(TcpClient client, TicketCalculator calculator)
        {
            _client = client;
            _calculator = calculator;
        }

        public async Task ProcessAsync()
        {
            try
            {
                using NetworkStream stream = _client.GetStream();
                byte[] buffer = new byte[8192];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string json = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);
                if (request?.Routes == null || request.Routes.Count == 0)
                {
                    await SendMessageAsync(stream, JsonSerializer.Serialize(new { Error = "Маршруты не переданы" }));
                    return;
                }
                Dictionary<string, decimal> prices = await _calculator.CalculatePricesAsync(request.Routes, request.Days);
                var minPair = prices.Where(p => p.Value >= 0).OrderBy(p => p.Value).First();
                var response = new FlightResponse
                {
                    MinPrice = minPair.Value,
                    MinDate = minPair.Key
                };
                if (request.IncludeAllPrices)
                {
                    response.Prices = prices;
                }
                string responseJson = JsonSerializer.Serialize(response);
                await SendMessageAsync(stream, responseJson);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка обработки клиента: {ex.Message}");
            }
            finally
            {
                _client.Close();
                Console.WriteLine("Клиент отключился");
            }
        }

        private async Task SendMessageAsync(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }
    }
}