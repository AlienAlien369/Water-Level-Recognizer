namespace WLR.Application.Common.Interfaces;
public interface ISignalRService
{
    Task SendMotorStatusUpdateAsync(Guid motorId, object payload, CancellationToken cancellationToken = default);
    Task SendDashboardUpdateAsync(Guid centerId, object payload, CancellationToken cancellationToken = default);
    Task SendNotificationAsync(Guid userId, object payload, CancellationToken cancellationToken = default);
}
