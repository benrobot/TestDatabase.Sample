using System;
using Microsoft.Extensions.Logging;

namespace TestDatabase.Sample.LoggingUtilities
{
    public class TestLogger : ILogger
    {
        private readonly Action<string> _logAction;

        public TestLogger(Action<string> logAction) => _logAction = logAction;
        

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => _logAction(formatter.Invoke(state, exception));
        

        public bool IsEnabled(LogLevel logLevel) => true;
        

        public IDisposable BeginScope<TState>(TState state) => new TestDisposable();
    }
}