using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Moq;
using WeatherApp;
using WeatherApp.Controllers;
using WeatherApp.Models;

namespace WeatherAPI.Tests
{
    public class WeatherAPITests
    {

        private readonly Mock<IWeatherService> _mockService;
        private readonly WeatherForecastController _controller;

        public WeatherAPITests()
        {
            _mockService = new Mock<IWeatherService>();
            _controller = new WeatherForecastController(Mock.Of<ILogger<WeatherForecastController>>(), _mockService.Object);
        }
        [Fact]
        public async void GetCurrentTemperature_ReturnsOK_Valid()
        {
            var response = new WeatherResponse
            {
                CurrentTemperature = 75,
                Unit = "F",
                Lat = 40.7128m,
                Lon = -74.0060m,
                RainPossibleToday = false
            };

            _mockService.Setup(s => s.GetCurrentWeatherByZipAsync("02472", "fahrenheit"))
                .ReturnsAsync(response);

            var result = await _controller.GetCurrentTemperatureAsync("02472", "fahrenheit");

            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<WeatherResponse>(okResult.Value);
            Assert.Equal(75, returnedResponse.CurrentTemperature);
            Assert.Equal("F", returnedResponse.Unit);
            Assert.Equal(40.7128m, returnedResponse.Lat);
            Assert.Equal(-74.0060m, returnedResponse.Lon);
            Assert.False(returnedResponse.RainPossibleToday);
        }

        [Theory]
        [InlineData("abc")] // Invalid ZIP
        [InlineData("021")]  // Too short
        public async Task GetCurrentTemperature_ReturnsBadRequest_WhenZipInvalid(string zip)
        {
            var result = await _controller.GetCurrentTemperatureAsync(zip, "celsius");
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrentTemperature_ReturnsNotFound_WhenLocationNotFound()
        {
            _mockService.Setup(s => s.GetCurrentWeatherByZipAsync("99999", "fahrenheit"))
                .ReturnsAsync((WeatherResponse)null);
            var result = await _controller.GetCurrentTemperatureAsync("99999", "fahrenheit");
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }

        [Fact]
        public async void GetCurrentTemperature_ReturnsBadRequest_WhenUnitsInvalid()
        {
            var result = await _controller.GetCurrentTemperatureAsync("02472", "kelvin");
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetCurrentTemperature_ReturnsInternalServerError_WhenExceptionThrown()
        {
            _mockService.Setup(s => s.GetCurrentWeatherByZipAsync(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new Exception("Test exception"));
            var result = await _controller.GetCurrentTemperatureAsync("02472", "fahrenheit");
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error.", objectResult.Value);
        }

        [Fact]
        public async Task GetAverageTemperature_ReturnsOK_Valid()
        {
            var response = new WeatherAverageResponse
            {
                AverageTemperature = 70,
                Unit = "F",
                Lat = 40.7128m,
                Lon = -74.0060m,
                RainPossibleInPeriod = false

            };
            _mockService.Setup(s => s.GetAverageTemperatureByZipAsync("02472", "fahrenheit", 5))
                .ReturnsAsync(response);
            var result = await _controller.GetAverageTemperatureAsync("02472", "fahrenheit", 5);
            var okResult = Assert.IsType<OkObjectResult>(result.Result);
            var returnedResponse = Assert.IsType<WeatherAverageResponse>(okResult.Value);
            Assert.Equal(70, returnedResponse.AverageTemperature);
            Assert.Equal("F", returnedResponse.Unit);
            Assert.Equal(40.7128m, returnedResponse.Lat);
            Assert.Equal(-74.0060m, returnedResponse.Lon);
            Assert.False(returnedResponse.RainPossibleInPeriod);

        }

        [Theory]
        [InlineData("abc")] // Invalid ZIP
        [InlineData("021")]  // Too short
        public async Task GetAverageTemperature_ReturnsBadRequest_WhenZipInvalid(string zip)
        {
            var result = await _controller.GetAverageTemperatureAsync(zip, "celsius", 5);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAverageTemperature_ReturnsNotFound_WhenLocationNotFound()
        {
            _mockService.Setup(s => s.GetAverageTemperatureByZipAsync("99999", "fahrenheit", 5))
                .ReturnsAsync((WeatherAverageResponse)null);
            var result = await _controller.GetAverageTemperatureAsync("99999", "fahrenheit", 5);
            Assert.IsType<NotFoundObjectResult>(result.Result);
        }
        [Fact]
        public async Task GetAverageTemperature_ReturnsInternalServerError_WhenExceptionThrown()
        {
            _mockService.Setup(s => s.GetAverageTemperatureByZipAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<int>()))
                .Throws(new Exception("Test exception"));
            var result = await _controller.GetAverageTemperatureAsync("02472", "fahrenheit", 5);
            Assert.IsType<ObjectResult>(result.Result);
            var objectResult = result.Result as ObjectResult;
            Assert.Equal(500, objectResult.StatusCode);
            Assert.Equal("Internal server error.", objectResult.Value);
        }
    
        [Fact]
        public async void GetAverageTemperature_ReturnsBadRequest_WhenUnitsInvalid()
        {
            var result = await _controller.GetAverageTemperatureAsync("02472", "kelvin", 5);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }

        [Fact]
        public async Task GetAverageTemperature_ReturnsBadRequest_WhenTimePeriodInvalidLower()
        {
            var result = await _controller.GetAverageTemperatureAsync("02472", "fahrenheit", -1);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }
        [Fact]
        public async Task GetAverageTemperature_ReturnsBadRequest_WhenTimePeriodInvalidUpper()
        {
            var result = await _controller.GetAverageTemperatureAsync("02472", "fahrenheit", 6);
            Assert.IsType<BadRequestObjectResult>(result.Result);
        }


    }
}