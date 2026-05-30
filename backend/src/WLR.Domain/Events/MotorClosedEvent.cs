using WLR.Domain.Common;
namespace WLR.Domain.Events;
public record MotorClosedEvent(Guid MotorId, Guid OperatedByUserId, DateTime ClosedAt, double DurationMinutes) : IDomainEvent;
