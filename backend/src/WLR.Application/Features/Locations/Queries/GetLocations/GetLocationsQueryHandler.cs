using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Locations.Queries.GetLocations;

public class GetLocationsQueryHandler : IRequestHandler<GetLocationsQuery, PaginatedResult<LocationDto>>
{
    private readonly IApplicationDbContext _context;
    public GetLocationsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<LocationDto>> Handle(GetLocationsQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Locations.Include(l => l.Center).Where(l => !l.IsDeleted);

        if (request.CenterId.HasValue)
            q = q.Where(l => l.CenterId == request.CenterId.Value);

        if (!string.IsNullOrWhiteSpace(request.Params.Search))
            q = q.Where(l => l.Name.Contains(request.Params.Search));

        if (request.Params.IsActive.HasValue)
            q = q.Where(l => l.IsActive == request.Params.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(l => l.CreatedAt)
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(l => new LocationDto(
                l.Id, l.CenterId, l.Center != null ? l.Center.Name : "",
                l.Name, l.Description, l.Floor, l.Zone, l.IsActive,
                l.Motors.Count(m => !m.IsDeleted),
                l.Motors.Count(m => !m.IsDeleted && m.IsActive),
                l.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<LocationDto>(items, total, request.Params.PageNumber, request.Params.PageSize);
    }
}
