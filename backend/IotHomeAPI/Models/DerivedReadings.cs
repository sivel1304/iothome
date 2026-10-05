using IotHomeAPI.Dtos;

public static class DerivedReadings
{
    public static List<ReadingDto> WithDerived(IEnumerable<ReadingDto> readings)
    {
        var list = readings.ToList();
        var volts = list.FirstOrDefault(r => r.Type == "battery_voltage");
        if (volts is not null)
            list.Add(new ReadingDto("battery_level", 50));
        return list;
    }
}