using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Users.Commands.ToggleUserActive;

public class ToggleUserActiveCommandHandler : IRequestHandler<ToggleUserActiveCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public ToggleUserActiveCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(ToggleUserActiveCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        if (request.Activate)
            user.Activate();
        else
            user.Deactivate();

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync(request.Activate ? "ActivateUser" : "DeactivateUser", "User", user.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
