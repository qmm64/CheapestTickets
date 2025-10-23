using CheapestTickets.Server.Models;
using CheapestTickets.Server.Services;
using CheapestTickets.Server.Models.Responses;
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
            string clientInfo = _client.Client.RemoteEndPoint?.ToString() ?? "неизвестный клиент";
            using NetworkStream stream = _client.GetStream();

            try
            {
                string json = await ReceiveMessageAsync(stream);
                Logger.Info("Получен запрос от клиента", clientInfo);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);
                if (request?.Routes == null || request.Routes.Count == 0)
                {
                    await SendErrorAsync(stream, "Маршруты не переданы на сервер");
                    Logger.Warning("Клиент отправил пустой запрос", clientInfo);
                    return;
                }
                var calculateResponse = await _calculator.CalculatePricesAsync(request.Routes, request.Days);
                if (!string.IsNullOrEmpty(calculateResponse.Error))
                {
                    await SendErrorAsync(stream, calculateResponse.Error);
                    Logger.Warning($"Ошибка вычисления: {calculateResponse.Error}", clientInfo);
                    return;
                }
                var validPrices = calculateResponse.prices.Where(p => p.Value >= 0).ToList();
                if (validPrices.Count == 0)
                {
                    await SendErrorAsync(stream, "Нет доступных маршрутов по заданным параметрам");
                    Logger.Warning("Не найдено подходящих маршрутов", clientInfo);
                    return;
                }
                var minPair = validPrices.OrderBy(p => p.Value).First();
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
                Logger.Info($"Результат отправлен клиенту (минимум {minPair.Value}₽, дата {minPair.Key})", clientInfo);
            }
            catch (Exception ex)
            {
                Logger.Error($"Ошибка обработки клиента: {ex.Message}", clientInfo);
            }
            finally
            {
                _client.Close();
                Logger.Info("Клиент отключился", clientInfo);
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

        private async Task SendErrorAsync(NetworkStream stream, string error)
        {
            var response = new FlightResponse
            {
                Error = error
            };
            await SendMessageAsync(stream, JsonSerializer.Serialize(response));
        }
    }
}
