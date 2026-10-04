namespace IotHomeAPI.Models;

public class Measurement
{
    public int Id { get; set; }
    public string ModuleId { get; set; } = "";
    public Module Module { get; set; } = null!;
    public DateTime Timestamp { get; set; }     // UTC, stamped by the server
    public List<Reading> Readings { get; set; } = [];
}