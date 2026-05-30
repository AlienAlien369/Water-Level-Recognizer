using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Centers.Commands.DeleteCenter;

public class DeleteCenterCommandHandler : IRequestHandler<DeleteCenterCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public DeleteCenterCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteCenterCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.Id);

        center.SoftDelete(_currentUser.UserId?.ToString() ?? "system");
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("DeleteCenter", "Center", center.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
