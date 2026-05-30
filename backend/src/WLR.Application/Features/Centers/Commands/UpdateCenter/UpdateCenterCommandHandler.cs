using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Centers.Commands.UpdateCenter;

public class UpdateCenterCommandHandler : IRequestHandler<UpdateCenterCommand, CenterDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateCenterCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<CenterDto> Handle(UpdateCenterCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.Centers
            .Include(c => c.Locations.Where(l => !l.IsDeleted))
                .ThenInclude(l => l.Motors.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(c => c.Id == request.Id && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.Id);

        center.Update(request.Name, request.Description, request.Address, request.City, request.State, request.Country, request.ContactPhone, request.ContactEmail);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("UpdateCenter", "Center", center.Id.ToString(), cancellationToken: cancellationToken);

        return new CenterDto(center.Id, center.Name, center.Description, center.Address, center.City, center.State, center.Country,
            center.ContactPhone, center.ContactEmail, center.IsActive,
            center.Locations.Count,
            center.Locations.SelectMany(l => l.Motors).Count(),
            center.CreatedAt, center.RequiresAssignment);
    }
}
