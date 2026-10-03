namespace IotHomeAPI.Models;

public class SensorPayload
{
    public required int Temperature { get; set; }
    public required int Humidity { get; set; }
}