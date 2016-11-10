namespace CircuitBreakerPattern
{
    public interface IEventLogger
    {
        void Critical(string message);

        void Exception(string message);

        void Info(string message);

        void Warning(string message);

        void Performance(string message);

        void UserAction(string username, string url, string ip);

        void SendEmail(string username, string info);
    }
}