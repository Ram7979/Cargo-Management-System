namespace CMS.FleetService.Application.DTOs;

public class AddVehicleRequest
{
    public string PlateNumber { get; set; } = string.Empty;
    public string GpsDeviceId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string FuelType { get; set; } = "Diesel";
    public decimal CapacityKg { get; set; }
    public decimal VolumeCbm { get; set; }
    public string? RegistrationCertUrl { get; set; }
    public string? InsuranceDocUrl { get; set; }
    public DateTime? NextServiceDate { get; set; }
}
