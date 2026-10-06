namespace IotHomeAPI.Dtos;

public record ModuleDto(
    string Id,
    string? Name,
    string? SensorType,
    int IntervalMs,
    DateTime LastSeen);
public record ReadingDto(string Type, double Value);

public record LatestMeasurementDto(
    string ModuleId,
    DateTime Timestamp,
    List<ReadingDto> Readings);

public record MeasurementDto(DateTime Timestamp, List<ReadingDto> Readings);