using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Enums;

namespace WLR.Infrastructure.Identity;

public class CurrentUser : ICurrentUser
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public CurrentUser(IHttpContextAccessor httpContextAccessor)
        => _httpContextAccessor = httpContextAccessor;

    private ClaimsPrincipal? Principal => _httpContextAccessor.HttpContext?.User;

    public Guid? UserId
    {
        get
        {
            var id = Principal?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public string? UserName => Principal?.FindFirst(ClaimTypes.Name)?.Value;
    public string? MobileNumber => Principal?.FindFirst(ClaimTypes.MobilePhone)?.Value;

    public UserRole? Role
    {
        get
        {
            var role = Principal?.FindFirst(ClaimTypes.Role)?.Value;
            return Enum.TryParse<UserRole>(role, out var r) ? r : null;
        }
    }

    public Guid? CenterId
    {
        get
        {
            var id = Principal?.FindFirst("centerId")?.Value;
            return Guid.TryParse(id, out var guid) ? guid : null;
        }
    }

    public bool IsAuthenticated => Principal?.Identity?.IsAuthenticated == true;
    public bool IsInRole(UserRole role) => Role == role;
    public bool IsSuperAdmin => Role == UserRole.SuperAdmin;
    public bool IsAdmin => Role == UserRole.Admin || Role == UserRole.SuperAdmin;

    public string? IpAddress => _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString();
    public string? UserAgent => _httpContextAccessor.HttpContext?.Request.Headers["User-Agent"].ToString();
}
