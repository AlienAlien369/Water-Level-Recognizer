using WLR.Domain.Common;
using WLR.Domain.Enums;

namespace WLR.Domain.Entities;

public sealed class Notification : BaseEntity
{
    public Guid? UserId { get; private set; }
    public User? User { get; private set; }
    public Guid? CenterId { get; private set; }
    public string Title { get; private set; } = string.Empty;
    public string Message { get; private set; } = string.Empty;
    public NotificationType Type { get; private set; } = NotificationType.Info;
    public bool IsRead { get; private set; } = false;
    public DateTime? ReadAt { get; private set; }
    public string? EntityType { get; private set; }
    public string? EntityId { get; private set; }

    private Notification() { }

    public static Notification Create(string title, string message, NotificationType type, Guid? userId = null, Guid? centerId = null, string? entityType = null, string? entityId = null)
    {
        return new Notification
        {
            Title = title,
            Message = message,
            Type = type,
            UserId = userId,
            CenterId = centerId,
            EntityType = entityType,
            EntityId = entityId
        };
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
    }
}
