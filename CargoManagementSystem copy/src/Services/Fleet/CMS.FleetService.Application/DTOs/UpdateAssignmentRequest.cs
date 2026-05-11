namespace CMS.FleetService.Application.DTOs;

public class UpdateAssignmentRequest
{
    public DateTime? ScheduledPickup { get; set; }
    public Guid? VehicleId { get; set; }
    public string? Notes { get; set; }
}
