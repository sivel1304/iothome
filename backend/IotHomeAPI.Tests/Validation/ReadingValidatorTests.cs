using Xunit;
using IotHomeAPI.Validation;

public class ReadingValidatorTests
{
    [Theory]
    [InlineData("temperature", 22)]   // typical
    [InlineData("temperature", 0)]    // lower bound
    [InlineData("temperature", 50)]   // upper bound
    [InlineData("humidity", 20)]      // lower bound
    [InlineData("humidity", 90)]      // upper bound
    [InlineData("Temperature", 22)]   // type is case-insensitive
    [InlineData("soil", 40)]          // unknown types are accepted
    [InlineData("pressure", 1013.25)]
    public void IsValid_ReturnsTrue_ForInRangeValues(string type, double value)
        => Assert.True(ReadingValidator.IsValid(type, value));

    [Theory]
    [InlineData("temperature", -1)]   // too low
    [InlineData("temperature", 51)]   // too high
    [InlineData("humidity", 19)]      // too low
    [InlineData("humidity", 91)]      // too high
    [InlineData("humidity", 0)]       // the all-zeros garbage reading
    [InlineData("temperature", 255)]  // garbage from a bad bit decode
    [InlineData("soil", double.NaN)]
    [InlineData("soil", double.PositiveInfinity)]
    public void IsValid_ReturnsFalse_ForOutOfRangeValues(string type, double value)
        => Assert.False(ReadingValidator.IsValid(type, value));
}
