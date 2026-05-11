namespace CMS.FleetService.Application.DTOs;

public class AssignmentDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid DriverId { get; set; }
    public Guid VehicleId { get; set; }
    public string Status { get; set; } = string.Empty;
    public DateTime AssignedAt { get; set; }
    public DateTime? CompletedAt { get; set; }
    public DateTime? ScheduledPickup { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? CancellationReason { get; set; }
}
