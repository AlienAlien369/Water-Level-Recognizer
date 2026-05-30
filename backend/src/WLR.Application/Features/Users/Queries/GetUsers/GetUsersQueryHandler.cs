using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;
namespace WLR.Application.Features.Users.Queries.GetUsers;

public class GetUsersQueryHandler : IRequestHandler<GetUsersQuery, PaginatedResult<UserDto>>
{
    private readonly IApplicationDbContext _context;
    public GetUsersQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
    {
        // Global query filter already applies !IsDeleted
        var q = _context.Users.AsQueryable();

        if (request.CenterId.HasValue)
            q = q.Where(u => u.CenterId == request.CenterId.Value);

        if (!string.IsNullOrWhiteSpace(request.Params.Search))
            q = q.Where(u => u.Name.Contains(request.Params.Search) || u.MobileNumber.Contains(request.Params.Search));

        if (request.Params.IsActive.HasValue)
            q = q.Where(u => u.IsActive == request.Params.IsActive.Value);

        var total = await q.CountAsync(cancellationToken);

        var userList = await q
            .OrderByDescending(u => u.CreatedAt)
            .Skip((request.Params.PageNumber - 1) * request.Params.PageSize)
            .Take(request.Params.PageSize)
            .ToListAsync(cancellationToken);

        var centerIds = userList.Where(u => u.CenterId.HasValue).Select(u => u.CenterId!.Value).Distinct().ToList();
        var centers = centerIds.Count > 0
            ? await _context.Centers.Where(c => centerIds.Contains(c.Id)).ToDictionaryAsync(c => c.Id, c => c.Name, cancellationToken)
            : new Dictionary<Guid, string>();

        var items = userList.Select(u => new UserDto(u.Id, u.Name, u.MobileNumber, u.Email, u.Role, u.IsActive, u.CenterId, u.LastLoginAt, u.ProfileImageUrl, u.IsLocked, u.CenterId.HasValue && centers.TryGetValue(u.CenterId.Value, out var cn) ? cn : null, u.CreatedAt)).ToList();

        return new PaginatedResult<UserDto>(items, total, request.Params.PageNumber, request.Params.PageSize);
    }
}

