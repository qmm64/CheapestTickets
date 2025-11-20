using CheapestTickets.Server.Database;
using CheapestTickets.Server.Models.Database;
using CheapestTickets.Shared.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;

namespace CheapestTickets.Server.Services
{
    internal static class RequestLogger
    {
        private static AppDbContext? _db;

        public static void Init(AppDbContext db)
        {
            _db = db;
        }

        public static void LogRequest(ClientContext client, int days, long durationMs, FlightResponse flightResponse)
        {
            try
            {
                if (_db != null)
                {
                    _db.UserRequests.Add(new UserRequestLog
                    {
                        ClientId = client.Id,
                        IpAddress = client.Ip,
                        Days = days,
                        MinPrice = flightResponse.MinPrice,
                        MinDate = flightResponse.MinDate,
                        DurationMs = durationMs,
                        Error = flightResponse.Error != null ? flightResponse.Error.Type.ToString() : "",
                    });
                    _db.SaveChanges();
                }
            }
            catch (Exception ex)
            {
                Logger.Info($"Не удалось записать запрос пользователя: {ex.Message}", Logger.Sources.DATABASE);
            }
        }
    }
}
