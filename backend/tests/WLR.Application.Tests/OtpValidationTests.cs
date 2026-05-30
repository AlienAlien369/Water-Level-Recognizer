using FluentAssertions;

namespace WLR.Application.Tests;

public class OtpValidationTests
{
    [Fact]
    public void MobileNumber_ValidFormat_ShouldPass()
    {
        var validNumbers = new[] { "+919876543210", "+11234567890", "+447911123456" };
        foreach (var number in validNumbers)
            System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?[1-9]\d{9,14}$")
                .Should().BeTrue($"{number} should be valid");
    }

    [Fact]
    public void MobileNumber_InvalidFormat_ShouldFail()
    {
        var invalidNumbers = new[] { "123", "abcdef", "", "0000000000" };
        foreach (var number in invalidNumbers)
            System.Text.RegularExpressions.Regex.IsMatch(number, @"^\+?[1-9]\d{9,14}$")
                .Should().BeFalse($"{number} should be invalid");
    }

    [Fact]
    public void OtpCode_SixDigits_ShouldBeValid()
    {
        var otp = "123456";
        otp.Length.Should().Be(6);
        int.TryParse(otp, out _).Should().BeTrue();
    }
}
