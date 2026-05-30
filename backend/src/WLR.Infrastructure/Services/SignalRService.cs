using Microsoft.AspNetCore.SignalR;
using WLR.Application.Common.Interfaces;

namespace WLR.Infrastructure.Services;

public class SignalRService : ISignalRService
{
    private readonly IHubContext<MonitoringHub> _hub;

    public SignalRService(IHubContext<MonitoringHub> hub) => _hub = hub;

    public async Task SendMotorStatusUpdateAsync(Guid motorId, object payload, CancellationToken cancellationToken = default)
        => await _hub.Clients.Group("motors").SendAsync("MotorStatusUpdated", payload, cancellationToken);

    public async Task SendDashboardUpdateAsync(Guid centerId, object payload, CancellationToken cancellationToken = default)
        => await _hub.Clients.Group($"center-{centerId}").SendAsync("DashboardUpdated", payload, cancellationToken);

    public async Task SendNotificationAsync(Guid userId, object payload, CancellationToken cancellationToken = default)
        => await _hub.Clients.Group($"user-{userId}").SendAsync("NotificationReceived", payload, cancellationToken);
}

public class MonitoringHub : Hub
{
    public async Task JoinMotorGroup() => await Groups.AddToGroupAsync(Context.ConnectionId, "motors");
    public async Task JoinCenterGroup(string centerId) => await Groups.AddToGroupAsync(Context.ConnectionId, $"center-{centerId}");
    public async Task JoinUserGroup(string userId) => await Groups.AddToGroupAsync(Context.ConnectionId, $"user-{userId}");
    public async Task LeaveMotorGroup() => await Groups.RemoveFromGroupAsync(Context.ConnectionId, "motors");
}
