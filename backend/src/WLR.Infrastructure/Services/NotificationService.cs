using WLR.Application.Common.Interfaces;
using WLR.Domain.Entities;
using WLR.Domain.Enums;

namespace WLR.Infrastructure.Services;

public class NotificationService : INotificationService
{
    private readonly IApplicationDbContext _context;
    private readonly ISignalRService _signalR;

    public NotificationService(IApplicationDbContext context, ISignalRService signalR)
    {
        _context = context;
        _signalR = signalR;
    }

    public async Task SendToUserAsync(Guid userId, string title, string message, NotificationType type, string? entityType = null, string? entityId = null, CancellationToken cancellationToken = default)
    {
        var notification = Notification.Create(title, message, type, userId: userId, entityType: entityType, entityId: entityId);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
        await _signalR.SendNotificationAsync(userId, new { notification.Id, notification.Title, notification.Message, notification.Type }, cancellationToken);
    }

    public async Task SendToCenterAsync(Guid centerId, string title, string message, NotificationType type, CancellationToken cancellationToken = default)
    {
        var notification = Notification.Create(title, message, type, centerId: centerId);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BroadcastAsync(string title, string message, NotificationType type, CancellationToken cancellationToken = default)
    {
        var notification = Notification.Create(title, message, type);
        _context.Notifications.Add(notification);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
