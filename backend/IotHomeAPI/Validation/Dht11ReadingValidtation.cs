namespace IotHomeAPI.Validation;

public static class ReadingValidator
{
    // Plausible range per measurement type. Types not listed here are accepted as-is.
    private static readonly Dictionary<string, (double Min, double Max)> Ranges = new(StringComparer.OrdinalIgnoreCase)
    {
        // DHT11 datasheet ranges
        ["temperature"] = (0, 50),
        ["humidity"] = (20, 90),
    };

    public static bool IsValid(string type, double value) =>
        double.IsFinite(value) &&
        (!Ranges.TryGetValue(type, out var range) || value >= range.Min && value <= range.Max);
}
