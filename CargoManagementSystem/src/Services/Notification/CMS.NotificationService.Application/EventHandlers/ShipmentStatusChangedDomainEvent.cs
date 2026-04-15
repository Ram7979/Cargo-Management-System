using CMS.Shared.Events;

namespace CMS.NotificationService.Application.EventHandlers;

public class ShipmentStatusChangedDomainEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public string CustomerId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public string RecipientEmail { get; }

    public ShipmentStatusChangedDomainEvent(
        Guid shipmentId,
        string trackingNumber,
        string customerId,
        string oldStatus,
        string newStatus,
        string recipientEmail)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CustomerId = customerId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        RecipientEmail = recipientEmail;
    }
}
