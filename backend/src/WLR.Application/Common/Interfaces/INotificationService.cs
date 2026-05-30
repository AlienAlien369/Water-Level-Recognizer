using WLR.Domain.Enums;
namespace WLR.Application.Common.Interfaces;
public interface INotificationService
{
    Task SendToUserAsync(Guid userId, string title, string message, NotificationType type, string? entityType = null, string? entityId = null, CancellationToken cancellationToken = default);
    Task SendToCenterAsync(Guid centerId, string title, string message, NotificationType type, CancellationToken cancellationToken = default);
    Task BroadcastAsync(string title, string message, NotificationType type, CancellationToken cancellationToken = default);
}
