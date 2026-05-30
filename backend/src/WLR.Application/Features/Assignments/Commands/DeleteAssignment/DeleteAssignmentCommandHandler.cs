using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Assignments.Commands.DeleteAssignment;

public class DeleteAssignmentCommandHandler : IRequestHandler<DeleteAssignmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;
    private readonly IAuditService _auditService;

    public DeleteAssignmentCommandHandler(IApplicationDbContext context, ICurrentUser currentUser, IAuditService auditService)
    {
        _context = context;
        _currentUser = currentUser;
        _auditService = auditService;
    }

    public async Task<bool> Handle(DeleteAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.Assignments.FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Assignment", request.Id);

        assignment.SoftDelete(_currentUser.UserId?.ToString() ?? "system");
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("DeleteAssignment", "Assignment", assignment.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
