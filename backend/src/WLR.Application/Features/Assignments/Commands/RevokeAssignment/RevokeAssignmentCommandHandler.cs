using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Enums;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Assignments.Commands.RevokeAssignment;

public class RevokeAssignmentCommandHandler : IRequestHandler<RevokeAssignmentCommand, bool>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public RevokeAssignmentCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<bool> Handle(RevokeAssignmentCommand request, CancellationToken cancellationToken)
    {
        var assignment = await _context.Assignments
            .Include(a => a.Motor)
            .FirstOrDefaultAsync(a => a.Id == request.Id && !a.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Assignment", request.Id);

        if (assignment.Status != AssignmentStatus.Active)
            throw new ConflictException("Assignment is not active and cannot be revoked.");

        assignment.Revoke();

        if (assignment.Motor != null)
            assignment.Motor.UnassignSewadaar();

        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("RevokeAssignment", "Assignment", assignment.Id.ToString(), cancellationToken: cancellationToken);
        return true;
    }
}
