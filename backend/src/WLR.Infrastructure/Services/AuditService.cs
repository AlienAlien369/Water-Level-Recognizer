using System.Text.Json;
using WLR.Application.Common.Interfaces;
using WLR.Domain.Entities;

namespace WLR.Infrastructure.Services;

public class AuditService : IAuditService
{
    private readonly IApplicationDbContext _context;
    private readonly ICurrentUser _currentUser;

    public AuditService(IApplicationDbContext context, ICurrentUser currentUser)
    {
        _context = context;
        _currentUser = currentUser;
    }

    public async Task LogAsync(string action, string entityType, string? entityId = null, object? oldValues = null, object? newValues = null, string? additionalInfo = null, CancellationToken cancellationToken = default)
    {
        var log = AuditLog.Create(
            _currentUser.UserId,
            _currentUser.UserName,
            action,
            entityType,
            entityId,
            oldValues != null ? JsonSerializer.Serialize(oldValues) : null,
            newValues != null ? JsonSerializer.Serialize(newValues) : null,
            _currentUser.IpAddress,
            _currentUser.UserAgent,
            additionalInfo
        );
        _context.AuditLogs.Add(log);
        await _context.SaveChangesAsync(cancellationToken);
    }
}
