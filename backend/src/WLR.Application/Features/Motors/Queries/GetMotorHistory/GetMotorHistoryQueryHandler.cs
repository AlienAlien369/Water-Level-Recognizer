using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.Common.Models;
using WLR.Application.DTOs;

namespace WLR.Application.Features.Motors.Queries.GetMotorHistory;

public class GetMotorHistoryQueryHandler : IRequestHandler<GetMotorHistoryQuery, PaginatedResult<MotorSessionDto>>
{
    private readonly IApplicationDbContext _context;
    public GetMotorHistoryQueryHandler(IApplicationDbContext context) => _context = context;

    public async Task<PaginatedResult<MotorSessionDto>> Handle(GetMotorHistoryQuery request, CancellationToken cancellationToken)
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

        // Use IgnoreQueryFilters so soft-deleted motors/locations/centers are still joined
        var q = _context.MotorLogs
            .AsQueryable();

        if (start.HasValue)
            q = q.Where(l => l.ActionTime >= start.Value);
        if (end.HasValue)
            q = q.Where(l => l.ActionTime <= end.Value);
        if (request.MotorId.HasValue)
            q = q.Where(l => l.MotorId == request.MotorId.Value);
        if (request.CenterId.HasValue)
        {
            var motorIds = await _context.Motors
                .Where(m => m.Location != null && m.Location.CenterId == request.CenterId.Value)
                .Select(m => m.Id)
                .ToListAsync(cancellationToken);
            q = q.Where(l => motorIds.Contains(l.MotorId));
        }

        // Project to a flat DTO in SQL — avoids global query filter null issues
        var allLogs = await q
            .OrderBy(l => l.MotorId)
            .ThenBy(l => l.ActionTime)
            .Select(l => new
            {
                l.Id,
                l.MotorId,
                MotorNumber = l.Motor == null ? "" : l.Motor.MotorNumber,
                LocationName = l.Motor == null || l.Motor.Location == null ? "" : l.Motor.Location.Name,
                CenterName = l.Motor == null || l.Motor.Location == null || l.Motor.Location.Center == null
                    ? "" : l.Motor.Location.Center.Name,
                l.OperatedByUserId,
                OperatedByUserName = l.OperatedByUser == null ? "" : l.OperatedByUser.Name,
                l.Action,
                l.ActionTime,
                l.DurationMinutes,
            })
            .ToListAsync(cancellationToken);

        // Pair Open → Close logs per motor into sessions in-memory
        var sessions = new List<MotorSessionDto>();
        var pendingOpens = new Dictionary<Guid, (string Id, Guid MotorId, string MotorNumber, string LocationName,
            string CenterName, Guid OpenedByUserId, string OpenedByUserName, DateTime OpenTime)>();

        foreach (var log in allLogs)
        {
            if (log.Action == "Open")
            {
                // Previous open without a close → add as still-running session
                if (pendingOpens.TryGetValue(log.MotorId, out var prev))
                {
                    sessions.Add(new MotorSessionDto(
                        prev.Id, prev.MotorId, prev.MotorNumber, prev.LocationName, prev.CenterName,
                        prev.OpenedByUserId, prev.OpenedByUserName, prev.OpenTime,
                        null, null, true));
                }

                pendingOpens[log.MotorId] = (log.Id.ToString(), log.MotorId, log.MotorNumber,
                    log.LocationName, log.CenterName, log.OperatedByUserId, log.OperatedByUserName, log.ActionTime);
            }
            else if (log.Action == "Close" && pendingOpens.TryGetValue(log.MotorId, out var openLog))
            {
                sessions.Add(new MotorSessionDto(
                    openLog.Id, openLog.MotorId, openLog.MotorNumber, openLog.LocationName, openLog.CenterName,
                    openLog.OpenedByUserId, openLog.OpenedByUserName, openLog.OpenTime,
                    log.ActionTime, log.DurationMinutes, false));
                pendingOpens.Remove(log.MotorId);
            }
        }

        // Remaining opens have no close yet = still running
        foreach (var p in pendingOpens.Values)
        {
            sessions.Add(new MotorSessionDto(
                p.Id, p.MotorId, p.MotorNumber, p.LocationName, p.CenterName,
                p.OpenedByUserId, p.OpenedByUserName, p.OpenTime,
                null, null, true));
        }

        sessions.Sort((a, b) => b.OpenTime.CompareTo(a.OpenTime));

        var total = sessions.Count;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        var paged = sessions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedResult<MotorSessionDto>(paged, total, pageNumber, pageSize);
    }
}
