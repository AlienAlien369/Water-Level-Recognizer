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

        // Load all logs in range (we need both Open and Close to pair them)
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

        var allLogs = await q
            .OrderBy(l => l.MotorId)
            .ThenBy(l => l.ActionTime)
            .ToListAsync(cancellationToken);

        // Pair Open → Close logs per motor into sessions
        var sessions = new List<MotorSessionDto>();
        // track pending opens: motorId → open log
        var pendingOpens = new Dictionary<Guid, WLR.Domain.Entities.MotorLog>();

        foreach (var log in allLogs)
        {
            if (log.Action == "Open")
            {
                // If there was already an unpaired open for this motor, treat it as still-running
                if (pendingOpens.TryGetValue(log.MotorId, out var prev))
                    sessions.Add(BuildSession(prev, null));

                pendingOpens[log.MotorId] = log;
            }
            else if (log.Action == "Close")
            {
                if (pendingOpens.TryGetValue(log.MotorId, out var openLog))
                {
                    sessions.Add(BuildSession(openLog, log));
                    pendingOpens.Remove(log.MotorId);
                }
                // Close without matching open in range — skip
            }
        }

        // Any unpaired opens = still running
        foreach (var openLog in pendingOpens.Values)
            sessions.Add(BuildSession(openLog, null));

        // Sort newest first, then paginate in-memory
        sessions.Sort((a, b) => b.OpenTime.CompareTo(a.OpenTime));

        var total = sessions.Count;
        var pageSize = request.PageSize > 0 ? request.PageSize : 20;
        var pageNumber = request.PageNumber > 0 ? request.PageNumber : 1;
        var paged = sessions.Skip((pageNumber - 1) * pageSize).Take(pageSize).ToList();

        return new PaginatedResult<MotorSessionDto>(paged, total, pageNumber, pageSize);
    }

    private static MotorSessionDto BuildSession(WLR.Domain.Entities.MotorLog openLog, WLR.Domain.Entities.MotorLog? closeLog)
    {
        return new MotorSessionDto(
            openLog.Id.ToString(),
            openLog.MotorId,
            openLog.Motor?.MotorNumber ?? "",
            openLog.Motor?.Location?.Name ?? "",
            openLog.Motor?.Location?.Center?.Name ?? "",
            openLog.OperatedByUserId,
            openLog.OperatedByUser?.Name ?? "",
            openLog.ActionTime,
            closeLog?.ActionTime,
            closeLog?.DurationMinutes,
            closeLog is null
        );
    }
}
