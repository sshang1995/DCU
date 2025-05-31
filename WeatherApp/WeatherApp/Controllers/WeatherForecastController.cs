using Microsoft.AspNetCore.Mvc;
using System.Reflection.Emit;
using WeatherApp.Models;
using WeatherApp.Services;

namespace WeatherApp.Controllers
{
    [ApiController]
    [Route("Weather")]
    public class WeatherForecastController : ControllerBase
    {

        private readonly ILogger<WeatherForecastController> _logger;
        private readonly IWeatherService _weatherService; 

        public WeatherForecastController(ILogger<WeatherForecastController> logger, IWeatherService weatherService)
        {
            _logger = logger;
            _weatherService = weatherService;
        }

        [HttpGet("Current/{zipCode}")]
        public async Task<ActionResult<WeatherResponse>> GetCurrentTemperatureAsync(string zipcode, [FromQuery] string units ="fahrenheit")
        {
            try
            {
                if (string.IsNullOrEmpty(zipcode))
                {
                    return BadRequest("Zip code cannot be empty.");
                }
                if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
                {
                    return BadRequest("Zip code must be 5 numeric digits.");
                }
                if (units.ToLower() != "fahrenheit" && units.ToLower() != "celsius")
                {
                    return BadRequest("Unit must be Fahrenheit or Celsius");
                }
                var response = await _weatherService.GetCurrentWeatherByZipAsync(zipcode, units.ToLower());
                if (response == null)
                {
                    return NotFound("Location cannot be found.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }

        }

        [HttpGet("Average/{zipCode}")]
        public async Task<ActionResult<WeatherResponse>> GetAverageTemperatureAsync(string zipcode, [FromQuery] string units, [FromQuery] int timePeriod)
        {

            try
            {
                if (string.IsNullOrEmpty(zipcode))
                {
                    return BadRequest("Zip code cannot be empty.");
                }
                if (zipcode.Length != 5 || !zipcode.All(char.IsDigit))
                {
                    return BadRequest("Zip code must be 5 numeric digits.");
                }
                if (units.ToLower() != "fahrenheit" && units.ToLower() != "celsius")
                {
                    return BadRequest("Unit must be Fahrenheit or Celsius");
                }
                if(timePeriod < 2 || timePeriod > 5)
                {
                    return BadRequest("Time period must be 2-5 days"); 
                }
                var response = await _weatherService.GetAverageTemperatureByZipAsync(zipcode, units.ToLower(), timePeriod);
                if (response == null)
                {
                    return NotFound("Location cannot be found.");
                }
                return Ok(response);
            }
            catch (Exception ex)
            {
                return StatusCode(500, "Internal server error.");
            }
        }
    }
}
