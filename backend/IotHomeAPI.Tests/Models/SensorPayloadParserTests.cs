using IotHomeAPI.Models;
using Xunit;

public class SensorPayloadParserTests
{
    [Fact]
    public void ValidPayload_IsParsed()
    {
        bool ok = SensorPayloadParser.TryParse("""{"temperature":22,"humidity":55}""", out var p);

        Assert.True(ok);
        Assert.Equal(22, p!.Temperature);
        Assert.Equal(55, p.Humidity);
    }

    [Fact]
    public void FieldNames_AreCaseInsensitive()
    {
        Assert.True(SensorPayloadParser.TryParse("""{"Temperature":22,"HUMIDITY":55}""", out _));
    }

    [Fact]
    public void ExtraFields_AreIgnored()
    {
        // lets you add e.g. battery voltage to the firmware before the backend knows about it
        Assert.True(SensorPayloadParser.TryParse("""{"temperature":22,"humidity":55,"battery":3.9}""", out _));
    }

    [Theory]
    [InlineData("""{"temp":22,"hum":55}""")]                         // wrong field names
    [InlineData("""{"temperature":22}""")]                           // missing humidity
    [InlineData("""{"humidity":55}""")]                              // missing temperature
    [InlineData("{}")]                                               // empty object
    [InlineData("""{"temperature":"22","humidity":"55"}""")]         // numbers as strings
    [InlineData("""{"temperature":"abc","humidity":55}""")]          // non-numeric string
    [InlineData("""{"temperature":22.5,"humidity":55}""")]           // decimal where int expected
    [InlineData("""{"temperature":null,"humidity":55}""")]           // explicit null
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