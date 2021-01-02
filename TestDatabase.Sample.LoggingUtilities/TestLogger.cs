using System;
using System.IO;
using Microsoft.Extensions.Logging;

namespace TestDatabase.Sample.LoggingUtilities
{
    public class TestLogger : ILogger
    {
        private readonly TextWriter _writer;

        public TestLogger(TextWriter writer)
        {
            _writer = writer;
        }

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception, Func<TState, Exception, string> formatter)
        {
            _writer.WriteLine(formatter.Invoke(state, exception));
        }

        public bool IsEnabled(LogLevel logLevel) => true;
        

        public IDisposable BeginScope<TState>(TState state)
        {
            return new TestDisposable();
        }
    }
}