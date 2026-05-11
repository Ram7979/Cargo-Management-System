namespace CMS.FleetService.Application.DTOs;

public class UpdateVehicleStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string? Reason { get; set; }
}
