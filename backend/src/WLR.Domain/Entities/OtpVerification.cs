using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class OtpVerification : BaseEntity
{
    public string MobileNumber { get; private set; } = string.Empty;
    public string OtpCode { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; } = false;
    public DateTime? UsedAt { get; private set; }
    public int AttemptCount { get; private set; } = 0;
    public string? Purpose { get; private set; }

    private OtpVerification() { }

    public static OtpVerification Create(string mobileNumber, string otpCode, string? purpose = "Login", int expiryMinutes = 10)
    {
        return new OtpVerification
        {
            MobileNumber = mobileNumber,
            OtpCode = otpCode,
            ExpiresAt = DateTime.UtcNow.AddMinutes(expiryMinutes),
            Purpose = purpose
        };
    }

    public bool IsValid() => !IsUsed && ExpiresAt > DateTime.UtcNow && AttemptCount < 3;

    public void MarkUsed()
    {
        IsUsed = true;
        UsedAt = DateTime.UtcNow;
    }

    public void IncrementAttempt() => AttemptCount++;
}
