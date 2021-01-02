using System;
using System.Linq;
using FluentAssertions;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using TestDatabase.Sample.LoggingUtilities;
using TestDatabase.Sample.WebApiWithEFCore.Controllers;
using TestDatabase.Sample.WebApiWithEFCore.Data;
using TestDatabase.SqlServerDocker;

namespace TestDatabase.Sample.WebApiWithEFCore.NUnit
{
    public class WeatherForecastControllerTests
    {
        private WeatherForecastController _controllerWithSqlServer;
        private ILoggerFactory _loggerFactory;
        private DbContextOptions<BloggingContext> _dbContextOptionsWithSqlServer;

        [OneTimeSetUp]
        public void OneTimeSetup()
        {
            var logger = new TestLogger(TestContext.Out);
            _loggerFactory = new TestLoggerFactory(logger);

            var dockerSqlServerDatabaseLogger = _loggerFactory.CreateLogger<SqlServerDockerDatabase>();
            var testDatabase = new SqlServerDockerDatabase(new SqlServerDockerDatabaseOptions(dockerSqlServerHostPort: 1456, stopDockerInstanceOnDispose: false), dockerSqlServerDatabaseLogger);
            var connectionString = testDatabase.GetConnectionString("Blogging");

            _dbContextOptionsWithSqlServer = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlServer(connectionString)
                .UseLoggerFactory(_loggerFactory)
                .Options;

            var dbContext = new BloggingContext(_dbContextOptionsWithSqlServer);
            PrePopulateDatabase(dbContext);
        }

        private static void PrePopulateDatabase(BloggingContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            var blog = new Blog
            {
                Url = "https://TestDatabaseBlog.io/789"
            };
            dbContext.Add(blog);
            dbContext.Add(new Post
            {
                Blog = blog,
                PublicationDateTime = DateTimeOffset.Now.AddDays(-7),
                Title = "My first post",
                Content = "The content of my first post"
            });
            dbContext.Add(new Post
            {
                Blog = blog,
                PublicationDateTime = DateTimeOffset.Now.AddDays(-3),
                Title = "My second post",
                Content = "The content of my second post"
            });
            dbContext.SaveChanges();
        }

        [SetUp]
        public void Setup()
        {
            var logger = _loggerFactory.CreateLogger<WeatherForecastController>();
            _controllerWithSqlServer = new WeatherForecastController(logger, new BloggingContext(_dbContextOptionsWithSqlServer));
        }

        [Test]
        public void SucceedsWithSqlServer()
        {
            var result = _controllerWithSqlServer.Get();

            var firstForecast = result.First();
            firstForecast.BlogSummaries.Should().HaveCount(1);
            firstForecast.BlogSummaries.First().PostCount.Should().Be(2);
            firstForecast.BlogSummaries.First().PostTitles.First().PublicationYear.Should().Be(DateTime.Now.AddDays(-3).Year);
        }

        [Test]
        public void FailsWithSqlite()
        {
            // Arrange
            var logger = _loggerFactory.CreateLogger<WeatherForecastController>();
            var connection = new SqliteConnection("DataSource=:memory:");
            connection.Open();
            using var command = connection.CreateCommand();
            command.CommandText = "PRAGMA FOREIGN_KEYS=OFF";
            command.ExecuteNonQuery();
            var dbContextOptionsWithSqlite = new DbContextOptionsBuilder<BloggingContext>()
                .UseSqlite(connection)
                .UseLoggerFactory(_loggerFactory)
                .Options;
            var dbContext = new BloggingContext(dbContextOptionsWithSqlite);
            PrePopulateDatabase(dbContext);
            var controllerWithSqlite = new WeatherForecastController(logger, dbContext);

            // Act
            // Fails with 'The LINQ expression ... could not be translated'
            // because Sqlite can't perform greater than / less than comparisons on DateTimeOffsets (because it stores it as a string)
            controllerWithSqlite
                .Invoking(x => x.Get())
                .Should()
                .ThrowExactly<System.InvalidOperationException>()
                .And.Message.Should().Be(@"The LINQ expression 'DbSet<Post>()
    .Where(p0 => EF.Property<Nullable<int>>(EntityShaperExpression: 
        EntityType: Blog
        ValueBufferExpression: 
            ProjectionBindingExpression: EmptyProjectionMember
        IsNullable: False
    , ""BlogId"") != null && object.Equals(
        objA: (object)EF.Property<Nullable<int>>(EntityShaperExpression: 
            EntityType: Blog
            ValueBufferExpression: 
                ProjectionBindingExpression: EmptyProjectionMember
            IsNullable: False
        , ""BlogId""), 
        objB: (object)EF.Property<Nullable<int>>(p0, ""BlogId"")))
    .Where(p0 => p0.PublicationDateTime > DateTimeOffset.Now.AddDays(-5))' could not be translated. Either rewrite the query in a form that can be translated, or switch to client evaluation explicitly by inserting a call to 'AsEnumerable', 'AsAsyncEnumerable', 'ToList', or 'ToListAsync'. See https://go.microsoft.com/fwlink/?linkid=2101038 for more information.");
        }
    }
}