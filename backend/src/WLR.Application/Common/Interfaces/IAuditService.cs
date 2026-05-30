namespace WLR.Application.Common.Interfaces;
public interface IAuditService
{
    Task LogAsync(string action, string entityType, string? entityId = null, object? oldValues = null, object? newValues = null, string? additionalInfo = null, CancellationToken cancellationToken = default);
}
