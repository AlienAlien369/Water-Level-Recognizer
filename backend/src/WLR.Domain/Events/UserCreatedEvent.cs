using WLR.Domain.Common;
using WLR.Domain.Enums;
namespace WLR.Domain.Events;
public record UserCreatedEvent(Guid UserId, string MobileNumber, UserRole Role) : IDomainEvent;
