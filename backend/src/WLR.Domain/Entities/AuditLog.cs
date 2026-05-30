using WLR.Domain.Common;

namespace WLR.Domain.Entities;

public sealed class AuditLog : BaseEntity
{
    public Guid? UserId { get; private set; }
    public string? UserName { get; private set; }
    public string Action { get; private set; } = string.Empty;
    public string EntityType { get; private set; } = string.Empty;
    public string? EntityId { get; private set; }
    public string? OldValues { get; private set; }
    public string? NewValues { get; private set; }
    public string? IpAddress { get; private set; }
    public string? UserAgent { get; private set; }
    public string? AdditionalInfo { get; private set; }
    public DateTime Timestamp { get; private set; } = DateTime.UtcNow;

    private AuditLog() { }

    public static AuditLog Create(Guid? userId, string? userName, string action, string entityType, string? entityId = null, string? oldValues = null, string? newValues = null, string? ipAddress = null, string? userAgent = null, string? additionalInfo = null)
    {
        return new AuditLog
        {
            UserId = userId,
            UserName = userName,
            Action = action,
            EntityType = entityType,
            EntityId = entityId,
            OldValues = oldValues,
            NewValues = newValues,
            IpAddress = ipAddress,
            UserAgent = userAgent,
            AdditionalInfo = additionalInfo
        };
    }
}
