using Nyx.Core.Diagnostics.Logging;

namespace Nyx.Host.Diagnostics
{
    public sealed class NyxLogger : INyxLogger
    {
        public void Log(string message)
        {
            Console.WriteLine($"[ Message ] {message}");
        }

        public void LogWarning(string message)
        {
            Console.WriteLine($"[ !Warning! ] {message}");
        }

        public void LogError(string message)
        {
            Console.WriteLine($"[ !ERROR! ] {message}");
        }

        public void LogError(Exception exception)
        {
            Console.WriteLine($"[ !ERROR! ] Exception: {exception.GetType().Name}, {exception.Message} || Stack: {exception.StackTrace}");
        }

        public void LogError(string message, Exception exception)
        {
            Console.WriteLine($"[ !ERROR! ] {message} || Exception: {exception.GetType().Name}, {exception.Message} || Stack: {exception.StackTrace}");
        }
    }
}
