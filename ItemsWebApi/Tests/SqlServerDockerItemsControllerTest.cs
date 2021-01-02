using System;
using Items;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using TestDatabase.SqlServerDocker;
using Xunit.Abstractions;

namespace Tests
{
    public class SqlServerDockerItemsControllerTest : ItemsControllerTest
    {
        public SqlServerDockerItemsControllerTest(ITestOutputHelper outputHelper)
            : base(
                new DbContextOptionsBuilder<ItemsContext>()
                    .UseSqlServer(GetConnectionString(outputHelper))
                    .Options)
        {
        }

        private static string GetConnectionString(ITestOutputHelper outputHelper)
        {
            var options = new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1458);
            var logger = new TestLoggerForSqlServerDockerDatabase(outputHelper);
            var testDatabase = new SqlServerDockerDatabase(options, logger);
            var connectionString = testDatabase.GetConnectionString("Items");
            return connectionString;
        }
    }

    public class TestLoggerForSqlServerDockerDatabase : ILogger<SqlServerDockerDatabase>
    {
        private readonly ITestOutputHelper _outputHelper;

        public TestLoggerForSqlServerDockerDatabase(ITestOutputHelper outputHelper) => _outputHelper = outputHelper;

        public void Log<TState>(LogLevel logLevel, EventId eventId, TState state, Exception exception,
            Func<TState, Exception, string> formatter) => _outputHelper.WriteLine(formatter(state, exception));
        
        public bool IsEnabled(LogLevel logLevel) => true;

        public IDisposable BeginScope<TState>(TState state) => throw new NotImplementedException();
    }
}