using CMS.FleetService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.FleetService.Domain.Entities;

public class Assignment : BaseEntity
{
    public Guid ShipmentId { get; private set; }
    public Guid DriverId { get; private set; }
    public Guid VehicleId { get; private set; }
    public AssignmentStatus Status { get; private set; }
    public DateTime AssignedAt { get; private set; }
    public DateTime? CompletedAt { get; private set; }
    public DateTime? ScheduledPickup { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public string? CancellationReason { get; private set; }

    private Assignment() { }

    public static Assignment Create(Guid shipmentId, Guid driverId, Guid vehicleId,
        DateTime? scheduledPickup, string notes = "")
    {
        return new Assignment
        {
            ShipmentId = shipmentId,
            DriverId = driverId,
            VehicleId = vehicleId,
            Status = AssignmentStatus.Active,
            AssignedAt = DateTime.UtcNow,
            ScheduledPickup = scheduledPickup,
            Notes = notes
        };
    }

    public void Complete()
    {
        Status = AssignmentStatus.Completed;
        CompletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Cancel(string reason = "")
    {
        Status = AssignmentStatus.Cancelled;
        CancellationReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateScheduledPickup(DateTime scheduledPickup)
    {
        ScheduledPickup = scheduledPickup;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateNotes(string notes)
    {
        Notes = notes;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SwapVehicle(Guid newVehicleId)
    {
        VehicleId = newVehicleId;
        UpdatedAt = DateTime.UtcNow;
    }
}
