using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Centers.Queries.GetCenterById;

public class GetCenterByIdQueryHandler : IRequestHandler<GetCenterByIdQuery, CenterDto>
{
    private readonly IApplicationDbContext _context;
    public GetCenterByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<CenterDto> Handle(GetCenterByIdQuery request, CancellationToken cancellationToken)
    {
        var c = await _context.Centers
            .Include(x => x.Locations.Where(l => !l.IsDeleted))
                .ThenInclude(l => l.Motors.Where(m => !m.IsDeleted))
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Center", request.Id);

        return new CenterDto(c.Id, c.Name, c.Description, c.Address, c.City, c.State, c.Country,
            c.ContactPhone, c.ContactEmail, c.IsActive,
            c.Locations.Count,
            c.Locations.SelectMany(l => l.Motors).Count(),
            c.CreatedAt);
    }
}
