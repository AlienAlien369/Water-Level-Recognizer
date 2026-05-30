using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Users.Commands.PromoteUser;

public class PromoteUserCommandHandler : IRequestHandler<PromoteUserCommand, UserDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public PromoteUserCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<UserDto> Handle(PromoteUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == request.CenterId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.CenterId);

        user.PromoteToAdmin(request.CenterId);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("PromoteUser", "User", user.Id.ToString(), newValues: new { CenterId = request.CenterId }, cancellationToken: cancellationToken);

        return new UserDto(user.Id, user.Name, user.MobileNumber, user.Email, user.Role, user.IsActive, user.CenterId, user.LastLoginAt, user.ProfileImageUrl, user.IsLocked, center.Name, user.CreatedAt);
    }
}

