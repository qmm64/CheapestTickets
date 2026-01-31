using CheapestTickets.Server.Models;
using CheapestTickets.Server.Models.Responses;
using CheapestTickets.Server.Services;
using CheapestTickets.Shared.Models;
using System.IO;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

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
            try
            {
                Logger.Info($"Клиент подключился: {_client.Ip}", Logger.Sources.CLIENT, _client);

                string json = await ReceiveMessageAsync(_client.Stream, _client.TokenSource.Token);
                var request = JsonSerializer.Deserialize<FlightRequest>(json);

                if (request == null || request.Routes.Count == 0)
                {
                    await SendErrorAsync(new AppError(ErrorType.InvalidRequest,"Маршруты не переданы"), _client.TokenSource.Token);
                    return;
                }

                Logger.Info($"Начат расчёт стоимости для {request.Routes.Count} маршрутов", Logger.Sources.CLIENT, _client);

                var response = await _calculator.CalculatePricesAsync(
                    request.Routes,
                    request.Days,
                    _client.TokenSource.Token
                );

                _client.TokenSource.Token.ThrowIfCancellationRequested();

                if (response.Error != null)
                {
                    await SendErrorAsync(response.Error, _client.TokenSource.Token);
                    return;
                }

                var valid = response.prices.Where(p => p.Value >= 0).ToList();
                if (valid.Count == 0)
                {
                    await SendErrorAsync(new AppError(ErrorType.NoData,"Нет доступных маршрутов"), _client.TokenSource.Token);
                    return;
                }

                var min = valid.OrderBy(p => p.Value).First();

                var msg = new FlightResponse
                {
                    MinPrice = min.Value,
                    MinDate = min.Key,
                    Prices = request.IncludeAllPrices ? response.prices : null
                };

                await SendMessageAsync(JsonSerializer.Serialize(msg), _client.TokenSource.Token);

                Logger.Info($"Отправлен результат: мин. цена {min.Value} руб ({min.Key})", Logger.Sources.CLIENT, _client);
            }
            catch (OperationCanceledException)
            {
                Logger.Info("Запрос отменён (клиент отключился)", Logger.Sources.CLIENT, _client);
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

        private async Task<string> ReceiveMessageAsync(NetworkStream stream, CancellationToken token)
        {
            //byte[] buffer = new byte[4096];
            //int bytes = await stream.ReadAsync(buffer, 0, buffer.Length, token);

            //if (bytes == 0)
            //{
            //    throw new OperationCanceledException("Client disconnected");
            //}

            //return Encoding.UTF8.GetString(buffer, 0, bytes);

            try
            {
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length, token);

                // Клиент корректно закрыл соединение
                if (bytesRead == 0)
                {
                    _client.Cancel();
                    throw new OperationCanceledException("Клиент отключился");
                }

                return Encoding.UTF8.GetString(buffer, 0, bytesRead);
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                _client.Cancel();
                throw;
            }
        }

        private async Task SendMessageAsync(string message, CancellationToken token)
        {
            //byte[] data = Encoding.UTF8.GetBytes(message);
            //await _client.Stream.WriteAsync(data, 0, data.Length, token);

            try
            {
                byte[] data = Encoding.UTF8.GetBytes(message);
                await _client.Stream.WriteAsync(data, 0, data.Length, token);
            }
            catch (Exception ex) when (ex is IOException or ObjectDisposedException)
            {
                _client.Cancel();
                throw;
            }
        }

        private Task SendErrorAsync(AppError error, CancellationToken token)
        {
            var response = new FlightResponse { Error = error };
            return SendMessageAsync(JsonSerializer.Serialize(response), token);
        }
    }
}
