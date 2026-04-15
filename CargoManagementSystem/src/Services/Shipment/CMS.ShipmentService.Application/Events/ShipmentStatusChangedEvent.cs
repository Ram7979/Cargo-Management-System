using CMS.Shared.Events;

namespace CMS.ShipmentService.Application.Events;

public class ShipmentStatusChangedEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public Guid CustomerId { get; }
    public string OldStatus { get; }
    public string NewStatus { get; }

    public ShipmentStatusChangedEvent(Guid shipmentId, string trackingNumber, Guid customerId, string oldStatus, string newStatus)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CustomerId = customerId;
        OldStatus = oldStatus;
        NewStatus = newStatus;
    }
}
