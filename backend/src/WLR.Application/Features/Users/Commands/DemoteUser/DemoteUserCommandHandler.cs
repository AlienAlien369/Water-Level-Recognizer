using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Users.Commands.DemoteUser;

public class DemoteUserCommandHandler : IRequestHandler<DemoteUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public DemoteUserCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<UserDto> Handle(DemoteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        user.DemoteToUser();
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("DemoteUser", "User", user.Id.ToString(), cancellationToken: cancellationToken);

        return new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl, user.IsLocked, null, user.CreatedAt);
    }
}

