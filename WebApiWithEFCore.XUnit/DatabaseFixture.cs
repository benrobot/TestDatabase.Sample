using System;
using System.Data;
using Microsoft.Extensions.Logging;
using TestDatabase.Abstractions;
using TestDatabase.Sample.LoggingUtilities;
using TestDatabase.SqlServerDocker;
using Xunit.Abstractions;

namespace WebApiWithEFCore.XUnit
{
    public class DatabaseFixture : IDisposable
    {
        private readonly ITestDatabase _databaseServer;
        public string DbConnectionString => _databaseServer.GetConnectionString("WebApiWeatherBlog");

        public DatabaseFixture(IMessageSink messageSink)
        {
            var options = new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1455);
            var logger = new TestLoggerForToXUnitMessageSink(messageSink);
            var loggerFactory = new TestLoggerFactory(logger);
            var sqlServerDockerDatabaseLogger = loggerFactory.CreateLogger<SqlServerDockerDatabase>();
            _databaseServer = new SqlServerDockerDatabase(options, sqlServerDockerDatabaseLogger);
        }

        public void Dispose()
        {
            _databaseServer?.Dispose();
            GC.SuppressFinalize(this);
        }
    }
}