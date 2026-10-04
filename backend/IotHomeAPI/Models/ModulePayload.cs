namespace IotHomeAPI.Models;
public class ModulePayload
{
    public required int Interval { get; set; }
    public required Dictionary<string, double> Readings { get; set; }
}