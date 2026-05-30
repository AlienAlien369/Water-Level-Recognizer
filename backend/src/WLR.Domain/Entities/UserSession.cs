using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class UserSession : BaseEntity
{
    public Guid UserId { get; private set; }
    public User? User { get; private set; }
    public string RefreshToken { get; private set; } = string.Empty;
    public string? DeviceInfo { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public DateTime ExpiresAt { get; private set; }
    public bool IsRevoked { get; private set; } = false;
    public DateTime? RevokedAt { get; private set; }
    public string? ReplacedByToken { get; private set; }

    private UserSession() { }

    public static UserSession Create(Guid userId, string refreshToken, DateTime expiresAt, string? deviceInfo, string? ipAddress, string? userAgent)
    {
        return new UserSession
        {
            UserId = userId,
            RefreshToken = refreshToken,
            ExpiresAt = expiresAt,
            DeviceInfo = deviceInfo,
            IpAddress = ipAddress,
            UserAgent = userAgent
        };
    }

    public void Revoke(string? replacedBy = null)
    {
        IsRevoked = true;
        RevokedAt = DateTime.UtcNow;
        ReplacedByToken = replacedBy;
    }

    public bool IsActive() => !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
