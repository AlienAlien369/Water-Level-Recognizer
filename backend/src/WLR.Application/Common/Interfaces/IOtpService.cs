namespace WLR.Application.Common.Interfaces;
public interface IOtpService
{
    string GenerateOtp();
    Task SendOtpAsync(string mobileNumber, string otp, CancellationToken cancellationToken = default);
}
