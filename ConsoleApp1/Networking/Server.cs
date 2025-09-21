using CheapestTickets.Server.Services;
using System.Net;
using System.Net.Sockets;

namespace CheapestVacationTickets.Server.Networking
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
            Console.WriteLine($"Сервер запущен на порту {_port}...");
            while (true)
            {
                try
                {
                    TcpClient client = await listener.AcceptTcpClientAsync();
                    Console.WriteLine("Клиент подключился!");
                    var handler = new ClientHandler(client, _calculator);
                    _ = handler.ProcessAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Ошибка при приёме клиента: {ex.Message}");
                }
            }
        }
    }
}