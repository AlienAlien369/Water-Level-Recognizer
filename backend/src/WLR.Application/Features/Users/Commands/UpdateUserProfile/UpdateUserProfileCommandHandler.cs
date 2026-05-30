using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Users.Commands.UpdateUserProfile;

public class UpdateUserProfileCommandHandler : IRequestHandler<UpdateUserProfileCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateUserProfileCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<UserDto> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        user.UpdateProfile(request.Name, request.Email, request.ProfileImageUrl);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("UpdateUserProfile", "User", user.Id.ToString(), cancellationToken: cancellationToken);

        string? centerName = null;
        if (user.CenterId.HasValue)
        {
            centerName = await _context.Centers
                .Where(c => c.Id == user.CenterId.Value)
                .Select(c => c.Name)
                .FirstOrDefaultAsync(cancellationToken);
        }

        return new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl, user.IsLocked, centerName, user.CreatedAt);
    }
}

