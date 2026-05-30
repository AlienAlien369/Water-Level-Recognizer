using WLR.Domain.Enums;
namespace WLR.Application.DTOs;

public record RegisterRequest(string Name, string MobileNumber, string Password, string? Email, Guid? CenterId);
public record LoginRequest(string MobileNumber, string Password);
public record RefreshTokenRequest(string RefreshToken);

public record AuthResponse(
    string AccessToken,
    string RefreshToken,
    DateTime ExpiresAt,
    UserDto User
);

public record UserDto(
    Guid Id,
    string Name,
    string MobileNumber,
    string? Email,
    UserRole Role,
    bool IsActive,
    Guid? CenterId,
    DateTime? LastLoginAt,
    string? ProfileImageUrl,
    bool IsLocked = false,
    string? CenterName = null,
    DateTime CreatedAt = default
);
