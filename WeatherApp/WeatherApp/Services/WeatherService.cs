using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using WeatherApp.Models;

namespace WeatherApp.Services
{
    public class WeatherService: IWeatherService
    {
        private readonly HttpClient _httpClient;
        private readonly string _apiKey;

        public WeatherService(HttpClient httpClient, IConfiguration configuration)
        {
            _httpClient = httpClient;
            _apiKey = configuration["OpenWeatherMap:ApiKey"];
        }

        public async Task<WeatherResponse?> GetCurrentWeatherByZipAsync(string zipCode, string units)
        {
            if (string.IsNullOrWhiteSpace(zipCode))
            {
                return null;
            }
            //  Fahrenheit use units=imperial, Celsius use units=metric
            var parameter = units == "fahrenheit" ? "imperial" : "metric"; 
            var url = $"https://api.openweathermap.org/data/2.5/weather?zip={zipCode}&appid={_apiKey}&units={parameter}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            return new WeatherResponse
            {
                CurrentTemperature = (int)root.GetProperty("main").GetProperty("temp").GetDecimal(),
                Unit = units == "fahrenheit" ?"F": "C",
                Lat = root.GetProperty("coord").GetProperty("lat").GetDecimal(),
                Lon = root.GetProperty("coord").GetProperty("lon").GetDecimal(),
                RainPossibleToday = root.TryGetProperty("rain", out var rain) && rain.TryGetProperty("1h", out _)
            };
        }

        public async Task<WeatherAverageResponse?> GetAverageTemperatureByZipAsync(string zipCode, string units, int timePeriod)
        {
            var parameter = units == "fahrenheit" ? "imperial" : "metric";
            var url = $"https://api.openweathermap.org/data/2.5/forecast?zip={zipCode}&appid={_apiKey}&units={parameter}";
            var response = await _httpClient.GetAsync(url);
            if (!response.IsSuccessStatusCode)
            {
                return null;
            }
            var content = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(content);
            var root = doc.RootElement;
            return new WeatherAverageResponse
            {
                AverageTemperature = (int)root.GetProperty("list").EnumerateArray()
                    .Take(timePeriod)
                    .Average(x => x.GetProperty("main").GetProperty("temp").GetDecimal()),
                Unit = units == "fahrenheit" ? "F" : "C",
                Lat = root.GetProperty("city").GetProperty("coord").GetProperty("lat").GetDecimal(),
                Lon = root.GetProperty("city").GetProperty("coord").GetProperty("lon").GetDecimal(),
                RainPossibleInPeriod = root.GetProperty("list").EnumerateArray()
                    .Take(timePeriod)
                    .Any(x => x.TryGetProperty("rain", out var rain) && rain.TryGetProperty("1h", out _))
            };
        }


    }
}
