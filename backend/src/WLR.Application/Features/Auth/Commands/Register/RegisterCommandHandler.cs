using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Auth.Commands.Register;

public class RegisterCommandHandler : IRequestHandler<RegisterCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;
    private readonly IPasswordService _passwordService;
    private readonly IAuditService _auditService;

    public RegisterCommandHandler(IApplicationDbContext context, ITokenService tokenService, IPasswordService passwordService, IAuditService auditService)
    {
        _context = context;
        _tokenService = tokenService;
        _passwordService = passwordService;
        _auditService = auditService;
    }

    public async Task<AuthResponse> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var exists = await _context.Users
            .AnyAsync(u => u.MobileNumber == request.MobileNumber && !u.IsDeleted, cancellationToken);

        if (exists)
            throw new ConflictException($"A user with mobile number {request.MobileNumber} already exists.");

        var user = User.Create(request.Name, request.MobileNumber, request.Email);
        user.SetPassword(_passwordService.Hash(request.Password));
        _context.Users.Add(user);

        var accessToken = _tokenService.GenerateAccessToken(user);
        var refreshToken = _tokenService.GenerateRefreshToken();
        var session = UserSession.Create(user.Id, refreshToken, DateTime.UtcNow.AddDays(30), request.DeviceInfo, request.IpAddress, request.UserAgent);
        _context.UserSessions.Add(session);

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("Register", "User", user.Id.ToString(), cancellationToken: cancellationToken);

        return new AuthResponse(
            accessToken,
            refreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl)
        );
    }
}
