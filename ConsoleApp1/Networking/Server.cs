using CheapestTickets.Server.Services;
using System.Net;
using System.Net.Sockets;

namespace CheapestTickets.Server.Networking
{
    internal class Server
    {
        private readonly int _port;
        private readonly TicketCalculator _calculator;

        public Server(int port, TicketCalculator calculator)
        {
            _port = port;
            _calculator = calculator;
        }

        public async Task StartAsync()
        {
            TcpListener listener = new TcpListener(IPAddress.Any, _port);
            listener.Start();
            Logger.Info($"Сервер запущен на порту {_port}");
            while (true)
            {
                try
                {
                    ClientContext client = new(await listener.AcceptTcpClientAsync());
                    Logger.Info($"Подключился клиент: {client.Ip}");
                    var handler = new ClientHandler(client, _calculator);
                    _ = handler.ProcessAsync();
                }
                catch (Exception ex)
                {
                    Logger.Info($"Ошибка при приёме клиента: {ex.Message}");
                }
            }
        }
    }
}
