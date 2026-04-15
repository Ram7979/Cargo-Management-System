using CMS.Shared.Events;

namespace CMS.ShipmentService.Application.Events;

public class ShipmentCreatedEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public string TrackingNumber { get; }
    public Guid CustomerId { get; }

    public ShipmentCreatedEvent(Guid shipmentId, string trackingNumber, Guid customerId)
    {
        ShipmentId = shipmentId;
        TrackingNumber = trackingNumber;
        CustomerId = customerId;
    }
}
