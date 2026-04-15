namespace CMS.FleetService.Application.DTOs;

public class AddMaintenanceLogRequest
{
    public string MaintenanceType { get; set; } = string.Empty;
    public DateTime ServiceDate { get; set; }
    public decimal MileageAtService { get; set; }
    public DateTime? NextServiceDate { get; set; }
    public decimal? NextServiceMileage { get; set; }
    public string ServiceProvider { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public decimal Cost { get; set; }
}
