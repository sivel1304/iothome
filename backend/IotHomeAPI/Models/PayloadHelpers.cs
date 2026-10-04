namespace IotHomeAPI.Models;
public class ModulePayload
{
    public required int Interval { get; set; }
    public required List<ReadingPayload> Readings { get; set; }
}

public class ReadingPayload
{
    public required string Type { get; set; }
    public required double Value { get; set; }
}