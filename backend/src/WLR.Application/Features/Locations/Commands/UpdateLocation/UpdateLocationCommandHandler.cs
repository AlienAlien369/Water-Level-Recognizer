using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Locations.Commands.UpdateLocation;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, LocationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public UpdateLocationCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<LocationDto> Handle(UpdateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = await _context.Locations
            .Include(l => l.Center)
            .Include(l => l.Motors.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(l => l.Id == request.Id && !l.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Location", request.Id);

        location.Update(request.Name, request.Description, request.Floor, request.Zone);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("UpdateLocation", "Location", location.Id.ToString(), cancellationToken: cancellationToken);

        return new LocationDto(location.Id, location.CenterId, location.Center?.Name ?? "", location.Name, location.Description,
            location.Floor, location.Zone, location.IsActive,
            location.Motors.Count, location.Motors.Count(m => m.IsActive), location.CreatedAt);
    }
}
