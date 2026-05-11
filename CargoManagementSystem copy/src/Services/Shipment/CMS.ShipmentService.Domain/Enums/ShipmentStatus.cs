namespace CMS.ShipmentService.Domain.Enums;

public enum ShipmentStatus
{
    Pending,
    Assigned,
    PickedUp,
    InTransit,
    AtWarehouse,
    OutForDelivery,
    Delivered,
    FailedDelivery,
    ReturnedToWarehouse,
    Cancelled,
    OnHold
}
