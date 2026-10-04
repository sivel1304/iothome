using IotHomeAPI.Models;
using Xunit;

public class SensorPayloadParserTests
{
    [Fact]
    public void ValidPayload_IsParsed()
    {
        bool ok = SensorPayloadParser.TryParse("""{"readings":{"temperature":22,"humidity":55},"interval":10}""", out var p);

        Assert.True(ok);
        Assert.Equal(22, p!.Readings["temperature"]);
        Assert.Equal(55, p.Readings["humidity"]);
        Assert.Equal(10, p.Interval);
    }

    [Fact]
    public void AnyMeasurementType_IsAccepted()
    {
        // new sensors only need a new key, no backend change
        bool ok = SensorPayloadParser.TryParse("""{"readings":{"soil":40,"pressure":1013.25},"interval":60}""", out var p);

        Assert.True(ok);
        Assert.Equal(40, p!.Readings["soil"]);
        Assert.Equal(1013.25, p.Readings["pressure"]);
    }

    [Fact]
    public void FieldNames_AreCaseInsensitive()
    {
        Assert.True(SensorPayloadParser.TryParse("""{"Readings":{"temperature":22},"INTERVAL":10}""", out _));
    }

    [Fact]
    public void ExtraFields_AreIgnored()
    {
        // lets you add e.g. battery voltage to the firmware before the backend knows about it
        Assert.True(SensorPayloadParser.TryParse("""{"readings":{"temperature":22},"interval":10,"battery":3.9}""", out _));
    }

    [Theory]
    [InlineData("""{"temperature":22,"humidity":55}""")]                                        // old flat format
    [InlineData("""{"readings":{"temperature":22,"humidity":55}}""")]                           // missing interval
    [InlineData("""{"interval":10}""")]                                                         // missing readings
    [InlineData("""{"readings":null,"interval":10}""")]                                         // null readings
    [InlineData("""{"readings":{},"interval":10}""")]                                           // empty readings
    [InlineData("""{"readings":[22,55],"interval":10}""")]                                      // readings not an object
    [InlineData("""{"readings":{"temperature":"22"},"interval":10}""")]                         // number as string
    [InlineData("""{"readings":{"temperature":"abc"},"interval":10}""")]                        // non-numeric string
    [InlineData("""{"readings":{"temperature":null},"interval":10}""")]                         // explicit null
    [InlineData("""{"readings":{"temperature":{"value":22}},"interval":10}""")]                 // nested object
    [InlineData("{}")]
    [InlineData("not json")]
    [InlineData("")]
    [InlineData("null")]
    [InlineData("[]")]
    public void BadPayload_ReturnsFalse_AndDoesNotThrow(string json)
    {
        bool ok = SensorPayloadParser.TryParse(json, out var p);

        Assert.False(ok);
        Assert.Null(p);
    }
}
