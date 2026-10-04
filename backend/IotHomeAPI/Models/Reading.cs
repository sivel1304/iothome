namespace IotHomeAPI.Models;

public class Reading
{
    public int Id { get; set; }
    public int MeasurementId { get; set; }
    public string Type { get; set; } = "";      // "temperature", "humidity", "battery"
    public double Value { get; set; }
}