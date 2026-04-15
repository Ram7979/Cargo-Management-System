using MediatR;

namespace CMS.Shared.Events;

/// <summary>
/// Marker interface for domain events. Extends MediatR INotification
/// so domain events can be dispatched via MediatR INotificationHandler.
/// </summary>
public interface IDomainEvent : INotification
{
}
