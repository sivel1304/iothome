namespace IotHomeAPI.Models;

public class DhtReading
{
    public int Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public int Temperature { get; set; }
    public int Humidity { get; set; }
    public DateTime Timestamp { get; set; }
}