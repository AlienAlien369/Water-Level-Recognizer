using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Assignments.Commands.CreateAssignment;

public class CreateAssignmentCommandHandler : IRequestHandler<CreateAssignmentCommand, AssignmentDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateAssignmentCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<AssignmentDto> Handle(CreateAssignmentCommand request, CancellationToken cancellationToken)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Id == request.UserId && !u.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("User", request.UserId);

        var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == request.CenterId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.CenterId);

        Location? location = null;
        if (request.LocationId.HasValue)
        {
            location = await _context.Locations.FirstOrDefaultAsync(l => l.Id == request.LocationId.Value && !l.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Location", request.LocationId.Value);
        }

        Motor? motor = null;
        if (request.MotorId.HasValue)
        {
            motor = await _context.Motors.FirstOrDefaultAsync(m => m.Id == request.MotorId.Value && !m.IsDeleted, cancellationToken)
                ?? throw new NotFoundException("Motor", request.MotorId.Value);

            motor.AssignSewadaar(request.UserId);
        }

        var assignment = Assignment.Create(request.UserId, request.CenterId, request.LocationId, request.MotorId, request.Notes);
        _context.Assignments.Add(assignment);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("CreateAssignment", "Assignment", assignment.Id.ToString(), newValues: new { request.UserId, request.CenterId }, cancellationToken: cancellationToken);

        return new AssignmentDto(assignment.Id, assignment.UserId,
            user.Name, user.MobileNumber,
            assignment.CenterId, center.Name,
            assignment.LocationId, location?.Name,
            assignment.MotorId, motor?.MotorNumber,
            assignment.Status, assignment.AssignedAt, assignment.RevokedAt, assignment.Notes, assignment.CreatedAt);
    }
}
