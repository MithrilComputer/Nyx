using Nyx.Core.Diagnostics.Logging;

namespace Nyx.Host.Diagnostics
{
    public sealed class NyxContextLogger<T> : INyxContextLogger<T>
    {

        private readonly INyxLogger inner;
        private readonly string context;

        public NyxContextLogger(INyxLogger inner)
        {
            this.inner = inner;
            context = typeof(T).Name;
        }

        public void Log(string message) =>
            inner.Log($"[{context}] {message}");

        public void LogWarning(string message) =>
            inner.LogWarning($"[{context}] {message}");

        public void LogError(string message) =>
            inner.LogError($"[{context}] {message}");

        public void LogError(Exception exception) =>
            inner.LogError($"[{context}]", exception);

        public void LogError(string message, Exception exception)
        {
            inner.LogError($"[{context}] {message}", exception);
        }
    }
}
