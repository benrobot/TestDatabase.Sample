using System;
using Microsoft.Extensions.Logging;
using Xunit.Abstractions;

namespace WebApiWithEFCore.XUnit
{
    public class TestLoggerToXUnitTestOutputHelper : ILogger
    {
        private readonly ITestOutputHelper _testOutputHelper;

        public TestLoggerToXUnitTestOutputHelper(ITestOutputHelper testOutputHelper) => _testOutputHelper = testOutputHelper;
        
        public IDisposable BeginScope<TState>(TState state) => new TestDisposable();

        public bool IsEnabled(LogLevel logLevel) => true;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => _testOutputHelper.WriteLine(formatter(state, exception));
    }
}