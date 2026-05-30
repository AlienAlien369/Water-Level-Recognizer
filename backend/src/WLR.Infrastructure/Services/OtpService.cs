using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using WLR.Application.Common.Interfaces;

namespace WLR.Infrastructure.Services;

public class OtpService : IOtpService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<OtpService> _logger;

    public OtpService(IConfiguration configuration, ILogger<OtpService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public string GenerateOtp()
    {
        using var rng = RandomNumberGenerator.Create();
        var bytes = new byte[4];
        rng.GetBytes(bytes);
        var number = BitConverter.ToUInt32(bytes, 0) % 1000000;
        return number.ToString("D6");
    }

    public async Task SendOtpAsync(string mobileNumber, string otp, CancellationToken cancellationToken = default)
    {
        var smsProvider = _configuration["SmsSettings:Provider"];

        // LogWarning so OTP appears in production logs (min level = Warning)
        _logger.LogWarning("📱 OTP for {MobileNumber}: {OTP} (provider={Provider})", mobileNumber, otp, smsProvider);

        if (smsProvider == "Console")
        {
            await Task.CompletedTask;
            return;
        }

        // TODO: Add actual SMS provider integration
        await Task.CompletedTask;
    }
}
