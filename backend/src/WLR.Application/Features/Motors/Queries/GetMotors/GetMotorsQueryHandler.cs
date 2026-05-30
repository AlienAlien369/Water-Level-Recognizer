using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Motors.Queries.GetMotors;

public class GetMotorsQueryHandler : IRequestHandler<GetMotorsQuery, PaginatedResult<MotorDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMotorsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<MotorDto>> Handle(GetMotorsQuery request, CancellationToken cancellationToken)
    {
        // Global query filter already applies !IsDeleted. Use Select() projection to avoid
        // null navigation properties caused by Include() + global query filter interaction.
        var q = _context.Motors.AsQueryable();

        if (request.LocationId.HasValue)
            q = q.Where(m => m.LocationId == request.LocationId.Value);

        if (request.CenterId.HasValue)
            q = q.Where(m => m.Location != null && m.Location.CenterId == request.CenterId.Value);

        if (!string.IsNullOrWhiteSpace(request.Params.Search))
            q = q.Where(m => m.MotorNumber.Contains(request.Params.Search));

        if (request.Params.IsActive.HasValue)
            q = q.Where(m => m.IsActive == request.Params.IsActive.Value);

        if (request.MinRunningHours.HasValue)
            q = q.Where(m => m.TotalRunningMinutes >= request.MinRunningHours.Value * 60);

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(m => m.CreatedAt)
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(m => new MotorDto(
                m.Id, m.LocationId,
                m.Location == null ? "" : m.Location.Name,
                m.Location == null ? Guid.Empty : m.Location.CenterId,
                m.Location == null || m.Location.Center == null ? "" : m.Location.Center.Name,
                m.MotorNumber, m.Description, m.WaterCapacityLiters,
                m.Status, m.CurrentState, m.LastOpenedAt, m.LastClosedAt,
                m.TotalRunningMinutes, m.IsActive,
                m.AssignedSewadaarId,
                m.AssignedSewadaar == null ? null : m.AssignedSewadaar.Name,
                m.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<MotorDto>(items, total, request.Params.PageNumber, request.Params.PageSize);
    }
}
