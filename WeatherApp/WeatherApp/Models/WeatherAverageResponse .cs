namespace WeatherApp
{
    public class WeatherAverageResponse
    {

        public int AverageTemperature { get; set; }
        public string Unit { get; set; } 
        public decimal Lat { get; set; }
        public decimal Lon { get; set; }
        public bool RainPossibleInPeriod { get; set; }

    }
}
