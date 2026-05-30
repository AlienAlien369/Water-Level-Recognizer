using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Centers.Queries.GetCenters;

public class GetCentersQueryHandler : IRequestHandler<GetCentersQuery, PaginatedResult<CenterDto>>
{
    private readonly IApplicationDbContext _context;
    public GetCentersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<CenterDto>> Handle(GetCentersQuery request, CancellationToken cancellationToken)
    {
        // Global query filter already applies !IsDeleted — no need to repeat it
        var q = _context.Centers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(request.Params.Search))
            q = q.Where(c => c.Name.Contains(request.Params.Search) || (c.City != null && c.City.Contains(request.Params.Search)));

        if (request.Params.IsActive.HasValue)
            q = q.Where(c => c.IsActive == request.Params.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(c => c.CreatedAt)
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(c => new CenterDto(
                c.Id, c.Name, c.Description, c.Address, c.City, c.State, c.Country,
                c.ContactPhone, c.ContactEmail, c.IsActive,
                c.Locations.Count(),
                c.Locations.Sum(l => (int?)l.Motors.Count()) ?? 0,
                c.CreatedAt, c.RequiresAssignment))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<CenterDto>(items, total, request.Params.PageNumber, request.Params.PageSize);
    }
}
