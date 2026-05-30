using WLR.Domain.Common;
namespace WLR.Domain.Events;
public record MotorOpenedEvent(Guid MotorId, Guid OperatedByUserId, DateTime OpenedAt) : IDomainEvent;
