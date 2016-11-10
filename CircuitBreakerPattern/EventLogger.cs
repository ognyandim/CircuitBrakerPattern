using System.Diagnostics;

namespace CircuitBreakerPattern
{
    public class EventLogger : IEventLogger
    {
        public void Critical(string message)
        {
            Trace.WriteLine(message);
        }

        public void Exception(string message)
        {
            Trace.WriteLine(message);
        }

        public void Info(string message)
        {
            Trace.WriteLine(message);
        }

        public void Warning(string message)
        {
            Trace.WriteLine(message);
        }

        public void Performance(string message)
        {
            Trace.WriteLine(message);
        }

        public void UserAction(string username, string url, string ip)
        {
            Trace.WriteLine($"username : {username} ; url {url} ; IP : {ip}");
        }

        public void SendEmail(string username, string info)
        {
            Trace.WriteLine($"username : {username} ; info {info}");
        }
    }
}