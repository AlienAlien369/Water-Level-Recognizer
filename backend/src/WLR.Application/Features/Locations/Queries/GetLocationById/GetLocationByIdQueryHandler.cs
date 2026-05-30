using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Locations.Queries.GetLocationById;

public class GetLocationByIdQueryHandler : IRequestHandler<GetLocationByIdQuery, LocationDto>
{
    private readonly IApplicationDbContext _context;
    public GetLocationByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<LocationDto> Handle(GetLocationByIdQuery request, CancellationToken cancellationToken)
    {
        var l = await _context.Locations
            .Include(x => x.Center)
            .Include(x => x.Motors.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Location", request.Id);

        return new LocationDto(l.Id, l.CenterId, l.Center?.Name ?? "", l.Name, l.Description, l.Floor, l.Zone,
            l.IsActive, l.Motors.Count, l.Motors.Count(m => m.IsActive), l.CreatedAt);
    }
}
