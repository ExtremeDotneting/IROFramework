
using System;

namespace Microsoft.Extensions.Logging
{
    public interface ILogger
    {
        void Log(LogLevel logLevel, params object[] messageArgs);

        bool IsEnabled(LogLevel logLevel);

        IDisposable BeginScope<TState>(TState state);
    }

    public interface ILogger<out TLoggerOwner>:ILogger
    {
    }
}
