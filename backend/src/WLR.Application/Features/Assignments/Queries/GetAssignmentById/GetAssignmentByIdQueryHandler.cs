using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Exceptions;
namespace WLR.Application.Features.Assignments.Queries.GetAssignmentById;

public class GetAssignmentByIdQueryHandler : IRequestHandler<GetAssignmentByIdQuery, AssignmentDto>
{
    private readonly IApplicationDbContext _context;
    public GetAssignmentByIdQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<AssignmentDto> Handle(GetAssignmentByIdQuery request, CancellationToken cancellationToken)
    {
        var a = await _context.Assignments
            .Include(x => x.User)
            .Include(x => x.Center)
            .Include(x => x.Location)
            .Include(x => x.Motor)
            .FirstOrDefaultAsync(x => x.Id == request.Id && !x.IsDeleted, cancellationToken)
            ?? throw new NotFoundException("Assignment", request.Id);

        return new AssignmentDto(a.Id, a.UserId,
            a.User?.Name ?? "", a.User?.MobileNumber ?? "",
            a.CenterId, a.Center?.Name ?? "",
            a.LocationId, a.Location?.Name,
            a.MotorId, a.Motor?.MotorNumber,
            a.Status, a.AssignedAt, a.RevokedAt, a.Notes, a.CreatedAt);
    }
}
