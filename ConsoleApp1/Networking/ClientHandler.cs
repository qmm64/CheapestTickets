using CheapestTickets.Server.Models;
using CheapestTickets.Server.Services;
using OpenQA.Selenium.Support.UI;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CheapestTickets.Server.Networking
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
            using NetworkStream stream = _client.GetStream();
            try
            {
                string json = await ReceiveMessageAsync(stream);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);
                if (request?.Routes == null || request.Routes.Count == 0)
                {
                    await SendErrorAsync(stream, "Маршруты не переданы на сервер");
                    return;
                }
                var calculateResponse = await _calculator.CalculatePricesAsync(request.Routes, request.Days);
                if (!string.IsNullOrEmpty(calculateResponse.Error))
                {
                    await SendErrorAsync(stream, calculateResponse.Error);
                    return;
                }
                var validPrices = calculateResponse.prices.Where(p => p.Value >= 0).ToList();
                if (validPrices.Count == 0)
                {
                    await SendErrorAsync(stream, "Нет доступных маршрутов по заданным параметрам");
                    return;
                }
                var minPair = validPrices.Where(p => p.Value >= 0).OrderBy(p => p.Value).First();
                var response = new FlightResponse
                {
                    MinPrice = minPair.Value,
                    MinDate = minPair.Key
                };
                if (request.IncludeAllPrices)
                {
                    response.Prices = calculateResponse.prices;
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

        private async Task<string> ReceiveMessageAsync(NetworkStream stream)
        {
            byte[] buffer = new byte[4096];
            int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
            return Encoding.UTF8.GetString(buffer, 0, bytesRead);
        }

        private async Task SendMessageAsync(NetworkStream stream, string message)
        {
            byte[] data = Encoding.UTF8.GetBytes(message);
            await stream.WriteAsync(data, 0, data.Length);
        }

        private async Task SendErrorAsync(NetworkStream stream,string error)
        {
            var response = new FlightResponse
            {
                Error = error
            };
            await SendMessageAsync(stream,JsonSerializer.Serialize(response));
        }
    }
}