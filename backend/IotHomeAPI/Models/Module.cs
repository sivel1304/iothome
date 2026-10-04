namespace IotHomeAPI.Models;
public class Module
{
    public string Id { get; set; } = "";        // "dht-alrum"
    public string? Name { get; set; }           // "Living room", editable later
    public string? SensorType { get; set; }
    public int IntervalMs { get; set; }
    public DateTime LastSeen { get; set; }
    public List<Measurement> Measurements { get; set; } = [];
}