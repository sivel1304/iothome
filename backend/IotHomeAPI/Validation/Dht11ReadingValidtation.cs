namespace IotHomeAPI.Validation;

public static class ReadingValidator
{
    // DHT11 datasheet ranges
    public const int MinTemp = 0, MaxTemp = 50;
    public const int MinHumidity = 20, MaxHumidity = 90;

    public static bool IsValid(int temperature, int humidity) =>
        temperature is >= MinTemp and <= MaxTemp &&
        humidity is >= MinHumidity and <= MaxHumidity;
}