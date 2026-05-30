using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Locations.Commands.DeleteLocation;

public class DeleteLocationCommandHandler : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public DeleteLocationCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Location", request.Id);

        location.SoftDelete(_currentUser.UserId?.ToString() ?? "system");
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("DeleteLocation", "Location", location.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
