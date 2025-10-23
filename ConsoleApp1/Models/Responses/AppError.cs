namespace CheapestTickets.Server.Models.Responses
{
    internal class AppError
    {
        public ErrorType Type { get; }
        public string Message { get; }

        public AppError(ErrorType type, string message)
        {
            Type = type;
            Message = message;
        }

        public static AppError None => new(ErrorType.None, string.Empty);
        public static AppError ApiUnavailable(string msg) => new(ErrorType.ApiUnavailable, msg);
        public static AppError Timeout(string msg) => new(ErrorType.Timeout, msg);
        public static AppError NoData(string msg) => new(ErrorType.NoData, msg);
        public static AppError Internal(string msg) => new(ErrorType.Internal, msg);

        public override string ToString() => Message;
    }

    public enum ErrorType
    {
        None,
        NoData,
        ApiUnavailable,
        Timeout,
        InvalidRequest,
        Internal
    }
}
