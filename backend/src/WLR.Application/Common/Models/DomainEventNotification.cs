using MediatR;
using WLR.Domain.Common;

namespace WLR.Application.Common.Models;

/// <summary>
/// Bridges a domain event to MediatR's INotification so it can be
/// dispatched via IMediator without polluting the Domain layer.
/// </summary>
public class DomainEventNotification<T> : INotification where T : IDomainEvent
{
    public T DomainEvent { get; }
    public DomainEventNotification(T domainEvent) => DomainEvent = domainEvent;
}
