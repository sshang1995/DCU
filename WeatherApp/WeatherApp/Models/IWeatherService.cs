namespace WeatherApp.Models
{
    public interface IWeatherService
    {
        Task<WeatherResponse?> GetCurrentWeatherByZipAsync(string zipCode, string units);
        Task<WeatherAverageResponse?> GetAverageTemperatureByZipAsync(string zipCode, string units, int timePeriod);
    }
}
