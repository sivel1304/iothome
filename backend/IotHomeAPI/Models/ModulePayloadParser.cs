using System.Text.Json;

namespace IotHomeAPI.Models;

public static class ModulePayloadParser
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static bool TryParse(string json, out ModulePayload? message)
    {
        message = null;
        try
        {
            var parsed = JsonSerializer.Deserialize<ModulePayload>(json, Options);

            if (parsed is null || parsed.Interval <= 0) return false;

            if (parsed.Readings is null || parsed.Readings.Count is 0 or > 20) return false;

            if (parsed.Readings.Keys.Any(k => string.IsNullOrWhiteSpace(k) || k.Length > 30))
                return false;

            message = parsed;
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}