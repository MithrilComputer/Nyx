namespace Nyx.Core.Diagnostics.Logging
{
    public interface INyxLogger
    {
        void Log(string message);

        void LogWarning(string message);

        void LogError(string message);

        void LogError(string message, Exception exception);

        void LogError(Exception exception);
    }
}
