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
                    TcpClient tcp = await listener.AcceptTcpClientAsync();
                    var context = new ClientContext(tcp);

                    Logger.Info($"Подключился клиент: {context.Ip}");

                    var handler = new ClientHandler(context, _calculator);

                    _ = Task.Run(() => handler.ProcessAsync());
                }
                catch (Exception ex)
                {
                    Logger.Info($"Ошибка при приёме клиента: {ex.Message}");
                }
            }
        }
    }
}
