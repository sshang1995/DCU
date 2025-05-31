namespace WeatherApp.Models
{
    public class WeatherResponse
    {

        public int CurrentTemperature { get; set; }
        public string Unit { get; set; } 
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public bool RainPossibleToday { get; set; }

    }
}
