using CheapestTickets.Server.Models;
using CheapestTickets.Server.Models.Responses;
using CheapestTickets.Server.Services;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

namespace CheapestTickets.Server.Networking
{
    internal class ClientHandler
    {
        private readonly TcpClient _client;
        private readonly TicketCalculator _calculator;
        private readonly Guid _clientId;

        public ClientHandler(TcpClient client, TicketCalculator calculator)
        {
            _client = client;
            _calculator = calculator;
            _clientId = Guid.NewGuid(); // уникальный ID для каждого клиента
        }

        public async Task ProcessAsync()
        {
            string clientIp = (_client.Client.RemoteEndPoint as System.Net.IPEndPoint)?.Address.ToString() ?? "Unknown";
            Logger.Info($"Подключился клиент: {clientIp}", $"CLIENT {_clientId}");
            using NetworkStream stream = _client.GetStream();
            using var cts = new CancellationTokenSource();
            try
            {
                string json = await ReceiveMessageAsync(stream);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);
                if (request?.Routes == null || request.Routes.Count == 0)
                {
                    await SendErrorAsync(stream, AppError.Internal("Маршруты не переданы на сервер"));
                    return;
                }
                Logger.Info($"Начат расчёт стоимости для {request.Routes.Count} маршрутов", $"CLIENT {_clientId}", clientIp);
                var calculateResponse = await _calculator.CalculatePricesAsync(request.Routes, request.Days, cts.Token);
                if (calculateResponse.Error != null)
                {
                    await SendErrorAsync(stream, calculateResponse.Error);
                    return;
                }
                var validPrices = calculateResponse.prices?.Where(p => p.Value >= 0).ToList() ?? new List<KeyValuePair<string, decimal>>();
                if (!validPrices.Any())
                {
                    await SendErrorAsync(stream, AppError.NoData("Нет доступных маршрутов по заданным параметрам"));
                    return;
                }

                var minPair = validPrices.OrderBy(p => p.Value).First();
                var response = new FlightResponse
                {
                    MinPrice = minPair.Value,
                    MinDate = minPair.Key
                };

                if (request.IncludeAllPrices)
                    response.Prices = calculateResponse.prices;

                string responseJson = JsonSerializer.Serialize(response);
                await SendMessageAsync(stream, responseJson);

                Logger.Info($"Отправлен результат: мин. цена {minPair.Value} руб ({minPair.Key})",
                            $"CLIENT {_clientId}", clientIp);
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка обработки клиента: {ex.Message}", $"CLIENT {_clientId}", clientIp);
            }
            finally
            {
                _client.Close();
                Logger.Info("Клиент отключился", $"CLIENT {_clientId}", clientIp);
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

        private async Task SendErrorAsync(NetworkStream stream, AppError error)
        {
            var response = new FlightResponse { Error = error };
            string responseJson = JsonSerializer.Serialize(response);
            await SendMessageAsync(stream, responseJson);
        }
    }
}
