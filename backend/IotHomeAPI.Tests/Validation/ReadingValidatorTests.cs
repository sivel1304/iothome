using Xunit;
using IotHomeAPI.Validation;

public class ReadingValidatorTests
{
    [Theory]
    [InlineData(22, 55)]   // typical
    [InlineData(0, 20)]    // lower bounds
    [InlineData(50, 90)]   // upper bounds
    public void IsValid_ReturnsTrue_ForInRangeValues(int temp, int hum)
        => Assert.True(ReadingValidator.IsValid(temp, hum));

    [Theory]
    [InlineData(-1, 50)]   // temp too low
    [InlineData(51, 50)]   // temp too high
    [InlineData(25, 19)]   // humidity too low
    [InlineData(25, 91)]   // humidity too high
    [InlineData(0, 0)]     // the all-zeros garbage reading
    [InlineData(255, 255)] // garbage from a bad bit decode
    public void IsValid_ReturnsFalse_ForOutOfRangeValues(int temp, int hum)
        => Assert.False(ReadingValidator.IsValid(temp, hum));
}