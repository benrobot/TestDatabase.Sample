using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace WebApiWithEFCore.XUnit
{
    public class TestLoggerForToXUnitMessageSink : ILogger
    {
        private readonly IMessageSink _messageSink;

        public TestLoggerForToXUnitMessageSink(IMessageSink messageSink) => _messageSink = messageSink;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) =>
            _messageSink.OnMessage(new DiagnosticMessage(formatter(state, exception)));

        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}