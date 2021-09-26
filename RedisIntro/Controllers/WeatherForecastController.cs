using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RedisIntro.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherForecastController : ControllerBase
    {
        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IDistributedCache _cache;

        private IEnumerable<WeatherForecast> forecasts;
        private DateTime cacheDateTime = DateTime.Now;
        private string loadLocation = "";

        public WeatherForecastController(IServiceProvider serviceProvider)
        {
            _cache = serviceProvider.GetService<IDistributedCache>();
            _logger = serviceProvider.GetService<ILogger<WeatherForecastController>>();
        }

        private static readonly string[] Summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        private async Task LoadForecast(DateTime dateTime)
        {
            forecasts = null;
            loadLocation = null;

            string recordKey = $"RedisIntro_{dateTime.ToString("yyyyMMdd_hhmm")}";
            forecasts = await _cache.GetRecordAsync<IEnumerable<WeatherForecast>>(recordKey);

            if (forecasts is null)
            {
                cacheDateTime = DateTime.Now;
                forecasts = GetWeatherForecast(cacheDateTime);
                loadLocation = $"Loaded from API at {cacheDateTime}";

                recordKey = $"RedisIntro_{cacheDateTime.ToString("yyyyMMdd_hhmm")}";
                await _cache.SetRecordAsync(recordKey, forecasts);
            }
            else
            {
                loadLocation = $"Loaded from cache at {dateTime}";
            }

            _logger.LogInformation(loadLocation);
        }

        private IEnumerable<WeatherForecast> GetWeatherForecast(DateTime dateTime)
        {
            var rng = new Random();
            return Enumerable.Range(1, 5).Select(index => new WeatherForecast
            {
                Date = dateTime.AddDays(index),
                TemperatureC = rng.Next(-20, 55),
                Summary = Summaries[rng.Next(Summaries.Length)]
            });
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                await LoadForecast(cacheDateTime);
                return Ok(forecasts);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new
                {
                    ExceptionType = ex.GetType().Name,
                    ExceptionTypeFullName = ex.GetType().FullName,
                    ExceptionMessage = ex.Message
                });
            }
            
        }
    }
}
