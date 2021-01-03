using System;
using Microsoft.Extensions.Logging;
using TestDatabase.Abstractions;
using TestDatabase.Sample.LoggingUtilities;
using TestDatabase.SqlServerDocker;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace WebApiWithEFCore.XUnit
{
    public class DatabaseFixture : IDisposable
    {
        private readonly ITestDatabase _databaseServer;
        public string DbConnectionString => _databaseServer.GetConnectionString("WebApiWeatherBlog");

        public DatabaseFixture(IMessageSink messageSink)
        {
            var options = new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1455);
            var logger = new TestLogger(m => messageSink.OnMessage(new DiagnosticMessage(m)));
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