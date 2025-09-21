
namespace CheapestTickets
{
    internal static class RequestHandler
    {
        private static void ShowErrorMessage(string message)
        {
            Console.WriteLine($"Ошибка ввода: {message}");
        }

        public static int GetCount(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                if (int.TryParse(Console.ReadLine(), out int count) && count>0)
                {
                    return count;
                }
                ShowErrorMessage("Введённая строка не соответствует положительному числу");
            }
            
        }

        public static DateOnly GetDate(string message)
        {
            while (true)
            {
                Console.WriteLine(message);
                if (!DateOnly.TryParse(Console.ReadLine(), out DateOnly date))
                {
                    ShowErrorMessage("Введённая строка не соответствует формату 'yyyy-mm-dd'");
                }
                else if(date < DateOnly.FromDateTime(DateTime.Now.Date))
                {
                    ShowErrorMessage("Введённая дата должна быть равна или больше сегодняшней");
                }
                else
                {
                    return date;
                }   
            }
        }

        public static string GetAirportCode(string message)
        {
            while (true) 
            {
                Console.WriteLine(message);
                string code = Console.ReadLine()?.Trim().ToUpper();
                if (!string.IsNullOrEmpty(code) && code.Length == 3 && code.All(char.IsLetter))
                {
                    return code;
                }
                ShowErrorMessage("Код аэропорта должен состоять из 3 букв");
            }
        }
    }
}
