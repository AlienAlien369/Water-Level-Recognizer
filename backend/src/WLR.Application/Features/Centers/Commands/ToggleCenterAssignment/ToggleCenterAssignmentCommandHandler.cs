using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;

namespace WLR.Application.Features.Centers.Commands.ToggleCenterAssignment;

public class ToggleCenterAssignmentCommandHandler : IRequestHandler<ToggleCenterAssignmentCommand, CenterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public ToggleCenterAssignmentCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<CenterDto> Handle(ToggleCenterAssignmentCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.Centers
            .Include(c => c.Locations.Where(l => !l.IsDeleted))
                .ThenInclude(l => l.Motors.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == request.CenterId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.CenterId);

        center.SetRequiresAssignment(request.RequiresAssignment);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("ToggleCenterAssignment", "Center", center.Id.ToString(),
            newValues: new { RequiresAssignment = request.RequiresAssignment }, cancellationToken: cancellationToken);

        return new CenterDto(center.Id, center.Name, center.Description, center.Address, center.City, center.State,
            center.Country, center.ContactPhone, center.ContactEmail, center.IsActive,
            center.Locations.Count,
            center.Locations.SelectMany(l => l.Motors).Count(),
            center.CreatedAt, center.RequiresAssignment);
    }
}
