using CheapestTickets.Server.Services;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CheapestTickets.Shared.Models;
using System.Diagnostics;

namespace CheapestTickets.Server.Networking
{
    internal class ClientHandler
    {
        private readonly ClientContext _client;
        private readonly TicketCalculator _calculator;

        public ClientHandler(ClientContext client, TicketCalculator calculator)
        {
            _client = client;
            _calculator = calculator;
        }

        public async Task ProcessAsync()
        {
            Logger.Info($"Подключился клиент: {_client.Ip}", Logger.Sources.CLIENT, _client);
            var timer = Stopwatch.StartNew();
            try
            {
                string json = await ReceiveMessageAsync(_client.Stream);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);
                if (request?.Routes == null || request.Routes.Count == 0)
                {
                    await Error(_client.Stream,(request!=null ? request.Days : 0),timer, AppError.Internal("Маршруты не переданы на сервер"));
                    return;
                }
                Logger.Info($"Начат расчёт стоимости для {request.Routes.Count} маршрутов на {request.Days} дней",Logger.Sources.CLIENT, _client);
                var calculateResponse = await _calculator.CalculatePricesAsync(request.Routes, request.Days, _client.TokenSource.Token);
                if (calculateResponse.Error != null)
                {
                    await Error(_client.Stream, request.Days, timer, calculateResponse.Error);
                    return;
                }
                var validPrices = calculateResponse.prices?.Where(p => p.Value >= 0).ToList() ?? new List<KeyValuePair<string, decimal>>();
                if (!validPrices.Any())
                {
                    await Error(_client.Stream, request.Days, timer, AppError.NoData("Нет доступных маршрутов по заданным параметрам"));
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
                await SendMessageAsync(_client.Stream, responseJson);

                timer.Stop();
                Logger.Info($"Отправлен результат: мин. цена {minPair.Value} руб ({minPair.Key})",Logger.Sources.CLIENT, _client);
                RequestLogger.LogRequest(_client, request.Days,timer.ElapsedMilliseconds, response);
            }
            catch (Exception ex)
            {
                Logger.Info($"Ошибка обработки клиента: {ex.Message}", Logger.Sources.CLIENT, _client);
            }
            finally
            {
                _client.Cancel();
                Logger.Info("Клиент отключился", Logger.Sources.CLIENT, _client);
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

        private async Task SendErrorAsync(NetworkStream stream,FlightResponse response)
        {
            string responseJson = JsonSerializer.Serialize(response);
            await SendMessageAsync(stream, responseJson);
        }

        private async Task Error(NetworkStream stream, int days, Stopwatch timer, AppError appError)
        {
            var response = new FlightResponse{ Error = appError };
            timer.Stop();
            RequestLogger.LogRequest(_client, days, timer.ElapsedMilliseconds, response);
            await SendErrorAsync(stream, response);
        }
    }
}
