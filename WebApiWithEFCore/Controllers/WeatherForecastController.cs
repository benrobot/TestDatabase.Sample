using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using TestDatabase.Sample.WebApiWithEFCore.ApiModels;
using TestDatabase.Sample.WebApiWithEFCore.Data;

namespace TestDatabase.Sample.WebApiWithEFCore.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly BloggingContext _dbContext;

        public WeatherForecastController(ILogger<WeatherForecastController> logger, BloggingContext dbContext)
        {
            _logger = logger;
            _dbContext = dbContext;
        }

        [HttpGet]
        public IEnumerable<WeatherForecast> Get()
        {
            var blogSummaries = _dbContext.Blogs
                .Select(b => new BlogSummary
            {
                Url = b.Url,
                PostCount = b.Posts.Count,
                PostTitles = b.Posts
                    .Where(p => p.PublicationDateTime > DateTimeOffset.Now.AddDays(-5))
                    .Select(p => new PostTitle
                {
                    PostId = p.PostId,
                    PublicationYear = p.PublicationDateTime.Year,
                    Title = p.Title
                }).ToList()
            }).ToList();

        var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = DateTime.Now.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)],
                BlogSummaries = blogSummaries
            })
            .ToArray();
        }
    }
}
