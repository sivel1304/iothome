namespace IotHomeAPI.Models;

public class SensorReading
{
    public int Id { get; set; }
    public string SensorId { get; set; } = string.Empty;
    public string Measurement { get; set; } = string.Empty;
    public string Value { get; set; } = string.Empty;
    public DateTime Timestamp { get; set; }
}