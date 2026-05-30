using MediatR;
using Microsoft.EntityFrameworkCore;
using WLR.Application.Common.Interfaces;
using WLR.Application.DTOs;
using WLR.Domain.Enums;

namespace WLR.Application.Features.Dashboard.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler : IRequestHandler<GetDashboardSummaryQuery, DashboardSummaryDto>
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public GetDashboardSummaryQueryHandler(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task<DashboardSummaryDto> Handle(GetDashboardSummaryQuery request, CancellationToken cancellationToken)
    {
        var centerId = request.CenterId ?? (_currentUser.IsAdmin ? _currentUser.CenterId : null);

        var centersQuery = _context.Centers.Where(c => !c.IsDeleted);
        var locationsQuery = _context.Locations.Where(l => !l.IsDeleted);
        var motorsQuery = _context.Motors.Where(m => !m.IsDeleted);
        var usersQuery = _context.Users.Where(u => !u.IsDeleted);

        if (centerId.HasValue && !_currentUser.IsSuperAdmin)
        {
            locationsQuery = locationsQuery.Where(l => l.CenterId == centerId.Value);
            motorsQuery = motorsQuery.Where(m => m.Location!.CenterId == centerId.Value);
            usersQuery = usersQuery.Where(u => u.CenterId == centerId.Value || u.Role == UserRole.User);
        }

        var today = DateTime.UtcNow.Date;
        var todayRunningMinutes = await _context.MotorLogs
            .Where(ml => ml.ActionTime >= today && ml.Action == "Close" && ml.DurationMinutes.HasValue)
            .SumAsync(ml => ml.DurationMinutes!.Value, cancellationToken);

        return new DashboardSummaryDto(
            TotalCenters: await centersQuery.CountAsync(cancellationToken),
            TotalLocations: await locationsQuery.CountAsync(cancellationToken),
            TotalMotors: await motorsQuery.CountAsync(cancellationToken),
            TotalUsers: await usersQuery.CountAsync(cancellationToken),
            ActiveMotors: await motorsQuery.CountAsync(m => m.Status == MotorStatus.Active || m.Status == MotorStatus.Running, cancellationToken),
            RunningMotors: await motorsQuery.CountAsync(m => m.CurrentState == MotorState.On, cancellationToken),
            FaultMotors: await motorsQuery.CountAsync(m => m.Status == MotorStatus.Fault, cancellationToken),
            TotalSewadars: await usersQuery.CountAsync(u => u.Role == UserRole.User, cancellationToken),
            TotalRunningMinutesToday: todayRunningMinutes
        );
    }
}
