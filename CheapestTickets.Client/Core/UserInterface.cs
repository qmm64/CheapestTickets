using CheapestTickets.Client.Models;

namespace CheapestTickets.Client.Core
{
    internal class UserInterface
    {
        public void DisplayResponse(FlightResponse? response)
        {
            if (response == null)
            {
                Console.WriteLine("Не удалось получить ответ от сервера.");
                return;
            }
            if (response.Error!=null)
            {
                Console.WriteLine($"Ошибка: {response.Error}");
                return;
            }
            if (response.MinPrice == null || response.MinDate == null)
            {
                Console.WriteLine("Не получилось найти подходящие маршруты.");
                return;
            }
            Console.WriteLine($"Дешевле всего вылетать {response.MinDate}: {response.MinPrice} руб.");
        }
    }
}
