using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Motors.Queries.GetMotorById;

public class GetMotorByIdQueryHandler : IRequestHandler<GetMotorByIdQuery, MotorDto>
{
    private readonly IApplicationDbContext _context;
    public GetMotorByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<MotorDto> Handle(GetMotorByIdQuery request, CancellationToken cancellationToken)
    {
        var m = await _context.Motors
            .Include(x => x.Location).ThenInclude(l => l!.Center)
            .Include(x => x.AssignedSewadaar)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Motor", request.Id);

        return new MotorDto(m.Id, m.LocationId,
            m.Location?.Name ?? "", m.Location?.CenterId ?? Guid.Empty,
            m.Location?.Center?.Name ?? "",
            m.MotorNumber, m.Description, m.WaterCapacityLiters,
            m.Status, m.CurrentState, m.LastOpenedAt, m.LastClosedAt,
            m.TotalRunningMinutes, m.IsActive,
            m.AssignedSewadaarId, m.AssignedSewadaar?.Name,
            m.CreatedAt);
    }
}
