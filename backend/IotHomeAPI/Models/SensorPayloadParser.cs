using System.Text.Json;

namespace IotHomeAPI.Models;

public static class SensorPayloadParser
{
    private static readonly JsonSerializerOptions Options = new() { PropertyNameCaseInsensitive = true };

    public static bool TryParse(string json, out SensorPayload? payload)
    {
        payload = null;
        try
        {
            payload = JsonSerializer.Deserialize<SensorPayload>(json, Options);
            return payload is not null;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}