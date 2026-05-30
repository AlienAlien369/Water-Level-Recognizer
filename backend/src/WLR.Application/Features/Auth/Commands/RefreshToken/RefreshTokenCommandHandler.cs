using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Auth.Commands.RefreshToken;

public class RefreshTokenCommandHandler : IRequestHandler<RefreshTokenCommand, AuthResponse>
{
    private readonly IApplicationDbContext _context;
    private readonly ITokenService _tokenService;

    public RefreshTokenCommandHandler(IApplicationDbContext context, ITokenService tokenService)
    {
        _context = context;
        _tokenService = tokenService;
    }

    public async Task<AuthResponse> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
    {
        var session = await _context.UserSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s => s.RefreshToken == request.RefreshToken, cancellationToken);

        if (session == null || !session.IsActive())
            throw new DomainException("Invalid or expired refresh token.");

        var user = session.User!;
        if (!user.IsActive || user.IsDeleted)
            throw new DomainException("User account is inactive.");

        var newRefreshToken = _tokenService.GenerateRefreshToken();
        session.Revoke(newRefreshToken);

        var newSession = Domain.Entities.UserSession.Create(user.Id, newRefreshToken, DateTime.UtcNow.AddDays(30), session.DeviceInfo, request.IpAddress, session.UserAgent);
        _context.UserSessions.Add(newSession);

        var accessToken = _tokenService.GenerateAccessToken(user);
        await _context.SaveChangesAsync(cancellationToken);

        return new AuthResponse(
            accessToken,
            newRefreshToken,
            DateTime.UtcNow.AddHours(1),
            new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl)
        );
    }
}
