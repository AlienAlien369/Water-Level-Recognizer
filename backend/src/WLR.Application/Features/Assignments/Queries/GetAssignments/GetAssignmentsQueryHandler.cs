using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Assignments.Queries.GetAssignments;

public class GetAssignmentsQueryHandler : IRequestHandler<GetAssignmentsQuery, PaginatedResult<AssignmentDto>>
{
    private readonly IApplicationDbContext _context;
    public GetAssignmentsQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<AssignmentDto>> Handle(GetAssignmentsQuery request, CancellationToken cancellationToken)
    {
        var q = _context.Assignments
            .Include(a => a.User)
            .Include(a => a.Center)
            .Include(a => a.Location)
            .Include(a => a.Motor)
            .Where(a => !a.IsDeleted);

        if (request.CenterId.HasValue)
            q = q.Where(a => a.CenterId == request.CenterId.Value);

        if (request.UserId.HasValue)
            q = q.Where(a => a.UserId == request.UserId.Value);

        if (!string.IsNullOrWhiteSpace(request.Params.Search))
            q = q.Where(a => a.User != null && (a.User.Name.Contains(request.Params.Search) || a.User.MobileNumber.Contains(request.Params.Search)));

        var total = await q.CountAsync(cancellationToken);

        var items = await q
            .OrderByDescending(a => a.CreatedAt)
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .Select(a => new AssignmentDto(
                a.Id, a.UserId,
                a.User != null ? a.User.Name : "",
                a.User != null ? a.User.MobileNumber : "",
                a.CenterId,
                a.Center != null ? a.Center.Name : "",
                a.LocationId, a.Location != null ? a.Location.Name : null,
                a.MotorId, a.Motor != null ? a.Motor.MotorNumber : null,
                a.Status, a.AssignedAt, a.RevokedAt, a.Notes, a.CreatedAt))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<AssignmentDto>(items, total, request.Params.PageNumber, request.Params.PageSize);
    }
}
