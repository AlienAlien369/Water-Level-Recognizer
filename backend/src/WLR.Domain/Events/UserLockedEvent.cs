using WLR.Domain.Common;
namespace WLR.Domain.Events;
public record UserLockedEvent(Guid UserId, DateTime LockedUntil) : IDomainEvent;
