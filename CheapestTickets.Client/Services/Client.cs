using CheapestTickets.Client.Models;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using CheapestTickets.Shared.Models;

namespace CheapestTickets.Client.Services
{
    internal class Client
    {
        private readonly string _serverAddress;
        private readonly int _port;

        public Client(string serverAddress = "127.0.0.1", int port = 5000)
        {
            _serverAddress = serverAddress;
            _port = port;
        }

        public async Task<FlightResponse> SendRequestAsync(FlightRequest request)
        {
            try
            {
                using var client = new TcpClient();
                await client.ConnectAsync(_serverAddress, _port);
                using var stream = client.GetStream();
                string requestJson = JsonSerializer.Serialize(request);
                byte[] requestData = Encoding.UTF8.GetBytes(requestJson);
                await stream.WriteAsync(requestData, 0, requestData.Length);
                byte[] buffer = new byte[4096];
                int bytesRead = await stream.ReadAsync(buffer, 0, buffer.Length);
                string responseJson = Encoding.UTF8.GetString(buffer, 0, bytesRead);
                var response = JsonSerializer.Deserialize<FlightResponse>(responseJson);
                return response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка клиента: {ex.Message}");
                return null;
            }
        }
    }
}