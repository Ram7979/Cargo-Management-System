using CMS.Shared.Events;

namespace CMS.FleetService.Application.Events;

public class ShipmentAssignedEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public Guid DriverId { get; }
    public Guid VehicleId { get; }
    public Guid AssignmentId { get; }

    public ShipmentAssignedEvent(Guid shipmentId, Guid driverId, Guid vehicleId, Guid assignmentId)
    {
        ShipmentId = shipmentId;
        DriverId = driverId;
        VehicleId = vehicleId;
        AssignmentId = assignmentId;
    }
}
