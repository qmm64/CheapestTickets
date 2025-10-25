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

        public static AppError None() 
        { 
            return new AppError(ErrorType.None, string.Empty);
        }
        public static AppError ApiUnavailable(string msg)
        {
            return new AppError(ErrorType.ApiUnavailable, msg);
        }
        public static AppError Timeout(string msg)
        {
            return new AppError(ErrorType.Timeout, msg);
        }
        public static AppError NoData(string msg)
        {
            return new AppError(ErrorType.NoData, msg);
        }
        public static AppError Internal(string msg)
        {
            return new AppError(ErrorType.Internal, msg);
        }

        public override string ToString()
        {
            return Message;
        }
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
