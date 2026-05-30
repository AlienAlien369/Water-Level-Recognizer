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
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _auditService;
    private readonly ILogger<LoginCommandHandler> _logger;

    public LoginCommandHandler(IApplicationDbContext context, ITokenService tokenService, IPasswordService passwordService, IAuditService auditService, ILogger<LoginCommandHandler> logger)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _auditService = auditService;
        _logger = logger;
    }

    public async Task<AuthResponse> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users
            .FirstOrDefaultAsync(u => u.MobileNumber == request.MobileNumber && !u.IsDeleted, cancellationToken);

        if (user == null || string.IsNullOrEmpty(user.PasswordHash))
            throw new DomainException("Invalid mobile number or password.");

        if (!user.CanLogin())
            throw new DomainException("Account is locked or inactive. Please contact support.");

        if (!_passwordService.Verify(request.Password, user.PasswordHash))
        {
            user.RecordFailedLogin();
            await _context.SaveChangesAsync(cancellationToken);
            throw new DomainException("Invalid mobile number or password.");
        }

        user.RecordLogin();

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();

        var session = UserSession.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(30), request.DeviceInfo, request.IpAddress, request.UserAgent);
        _context.UserSessions.Add(session);

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Login", "User", user.Id.ToString(), additionalInfo: $"IP: {request.IpAddress}", cancellationToken: cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl)
        );
    }
}
