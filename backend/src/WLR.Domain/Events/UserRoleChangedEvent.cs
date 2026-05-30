using WLR.Domain.Common;
using WLR.Domain.Enums;
namespace WLR.Domain.Events;
public record UserRoleChangedEvent(Guid UserId, UserRole OldRole, UserRole NewRole) : IDomainEvent;
