using CMS.Shared.Events;

namespace CMS.NotificationService.Application.EventHandlers;

public class ShipmentStatusChangedDomainEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public string CustomerId { get; }
    public string CustomerName { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }
    public string RecipientEmail { get; }
    public string RecipientPhone { get; }
    public DateTime? EstimatedDelivery { get; }

    public ShipmentStatusChangedDomainEvent(
        Guid shipmentId,
        string trackingNumber,
        string customerId,
        string customerName,
        string oldStatus,
        string newStatus,
        string recipientEmail,
        string recipientPhone = "",
        DateTime? estimatedDelivery = null)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CustomerId = customerId;
        CustomerName = customerName;
        OldStatus = oldStatus;
        NewStatus = newStatus;
        RecipientEmail = recipientEmail;
        RecipientPhone = recipientPhone;
        EstimatedDelivery = estimatedDelivery;
    }
}
