using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Auth.Commands.Login;

public class LoginCommandHandler : IRequestHandler<LoginCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IAuditService _auditService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService, IAuditService auditService, ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var otp = await _context.OtpVerifications
            .Where(o => o.MobileNumber == request.MobileNumber && !o.IsUsed && o.ExpiresAt > DateTime.UtcNow)
            .OrderByDescending(o => o.CreatedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (otp == null || !otp.IsValid() || otp.OtpCode != request.OtpCode)
        {
            otp?.IncrementAttempt();
            if (otp != null) await _context.SaveChangesAsync(cancellationToken);
            throw new DomainException("Invalid or expired OTP.");
        }

        otp.MarkUsed();

        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber && !u.IsDeleted, cancellationToken);

        if (user == null)
        {
            user = User.Create(request.MobileNumber, request.MobileNumber, null);
            _context.Users.Add(user);
        }

        if (!user.CanLogin())
            throw new DomainException("Account is locked or inactive. Please contact support.");

        user.RecordLogin();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var session = UserSession.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(30), request.DeviceInfo, request.IpAddress, request.UserAgent);
        _context.UserSessions.Add(session);

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Login", "User", user.Id.ToString(), additionalInfo: $"Device: {request.DeviceInfo}, IP: {request.IpAddress}", cancellationToken: cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl)
        );
    }
}
