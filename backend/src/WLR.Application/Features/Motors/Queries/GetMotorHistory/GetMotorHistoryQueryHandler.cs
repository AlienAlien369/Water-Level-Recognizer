using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;

namespace WLR.Application.Features.Motors.Queries.GetMotorHistory;

public class GetMotorHistoryQueryHandler : IRequestHandler<GetMotorHistoryQuery, PaginatedResult<MotorHistoryLogDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMotorHistoryQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<MotorHistoryLogDto>> Handle(GetMotorHistoryQuery request, CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        DateTime? start = null;
        DateTime? end = null;

        switch (request.DateFilter?.ToLower())
        {
            case "today":
                start = now.Date;
                end = now.Date.AddDays(1).AddTicks(-1);
                break;
            case "yesterday":
                start = now.Date.AddDays(-1);
                end = now.Date.AddTicks(-1);
                break;
            case "7days":
                start = now.Date.AddDays(-6);
                end = now.Date.AddDays(1).AddTicks(-1);
                break;
            case "custom":
                start = request.StartDate;
                end = request.EndDate.HasValue ? request.EndDate.Value.Date.AddDays(1).AddTicks(-1) : null;
                break;
        }

        var q = _context.MotorLogs
            .Include(l => l.Motor).ThenInclude(m => m!.Location).ThenInclude(loc => loc!.Center)
            .Include(l => l.OperatedByUser)
            .AsQueryable();

        if (start.HasValue)
            q = q.Where(l => l.ActionTime >= start.Value);
        if (end.HasValue)
            q = q.Where(l => l.ActionTime <= end.Value);
        if (request.MotorId.HasValue)
            q = q.Where(l => l.MotorId == request.MotorId.Value);
        if (request.CenterId.HasValue)
            q = q.Where(l => l.Motor != null && l.Motor.Location != null && l.Motor.Location.CenterId == request.CenterId.Value);

        var total = await q.CountAsync(cancellationToken);
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;

        var items = await q
            .OrderByDescending(l => l.ActionTime)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .Select(l => new MotorHistoryLogDto(
                l.Id,
                l.MotorId,
                l.Motor != null ? l.Motor.MotorNumber : "",
                l.Motor != null && l.Motor.Location != null ? l.Motor.Location.Name : "",
                l.Motor != null && l.Motor.Location != null && l.Motor.Location.Center != null ? l.Motor.Location.Center.Name : "",
                l.OperatedByUserId,
                l.OperatedByUser != null ? l.OperatedByUser.Name : "",
                l.Action,
                l.ActionTime,
                l.DurationMinutes,
                l.Notes
            ))
            .ToListAsync(cancellationToken);

        return new PaginatedResult<MotorHistoryLogDto>(items, total, pageNumber, pageSize);
    }
}
