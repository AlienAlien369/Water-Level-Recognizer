using WLR.Domain.Enums;
namespace WLR.Application.Common.Interfaces;
public interface ICurrentUser
{
    Guid? UserId { get; }
    string? UserName { get; }
    string? MobileNumber { get; }
    UserRole? Role { get; }
    Guid? CenterId { get; }
    bool IsAuthenticated { get; }
    bool IsInRole(UserRole role);
    bool IsSuperAdmin { get; }
    bool IsAdmin { get; }
    string? IpAddress { get; }
    string? UserAgent { get; }
}
