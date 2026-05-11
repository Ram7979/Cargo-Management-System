namespace CMS.FleetService.Application.DTOs;

public class UpdateVehicleRequest
{
    public string? Make { get; set; }
    public string? Model { get; set; }
    public int? Year { get; set; }
    public string? FuelType { get; set; }
    public decimal? CapacityKg { get; set; }
    public decimal? VolumeCbm { get; set; }
    public string? GpsDeviceId { get; set; }
    public string? InsuranceDocUrl { get; set; }
}
