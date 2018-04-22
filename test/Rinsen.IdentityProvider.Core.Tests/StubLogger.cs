using Microsoft.Extensions.Logging;
using System;

namespace Rinsen.IdentityProvider
{
    public class StubLogger<T> : ILogger<T>
    {
        public IDisposable BeginScope<TState>(TState state)
        {
            return null;
        }

        public bool IsEnabled(LogLevel logLevel)
        {
            return false;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            
        }
    }

    public static class StubLogger
    {
        public static StubLogger<T> CreateLogger<T>()
        {
            return new StubLogger<T>();
        }
    }
}
