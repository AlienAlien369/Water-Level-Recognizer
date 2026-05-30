using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Entities;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Locations.Commands.CreateLocation;

public class CreateLocationCommandHandler : IRequestHandler<CreateLocationCommand, LocationDto>
{
    private readonly IApplicationDbContext _context;
    private readonly IAuditService _auditService;

    public CreateLocationCommandHandler(IApplicationDbContext context, IAuditService auditService)
    {
        _context = context;
        _auditService = auditService;
    }

    public async Task<LocationDto> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var center = await _context.Centers.FirstOrDefaultAsync(c => c.Id == request.CenterId && !c.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.CenterId);

        var location = Location.Create(request.CenterId, request.Name, request.Description, request.Floor, request.Zone);
        _context.Locations.Add(location);
        await _context.SaveChangesAsync(cancellationToken);
        await _auditService.LogAsync("CreateLocation", "Location", location.Id.ToString(), newValues: new { location.Name }, cancellationToken: cancellationToken);

        return new LocationDto(location.Id, location.CenterId, center.Name, location.Name, location.Description,
            location.Floor, location.Zone, location.IsActive, 0, 0, location.CreatedAt);
    }
}
