namespace CMS.FleetService.Application.DTOs;

public class CreateAssignmentRequest
{
    public Guid ShipmentId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public DateTime? ScheduledPickup { get; set; }
    public string? Notes { get; set; }
    public bool ForceAssign { get; set; } = false;
    public decimal ShipmentWeightKg { get; set; }
}
