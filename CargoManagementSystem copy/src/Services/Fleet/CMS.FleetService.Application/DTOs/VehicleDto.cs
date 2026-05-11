namespace CMS.FleetService.Application.DTOs;

public class VehicleDto
{
    public Guid Id { get; set; }
    public string PlateNumber { get; set; } = string.Empty;
    public string GpsDeviceId { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public string Make { get; set; } = string.Empty;
    public string Model { get; set; } = string.Empty;
    public int Year { get; set; }
    public string FuelType { get; set; } = string.Empty;
    public decimal CapacityKg { get; set; }
    public decimal VolumeCbm { get; set; }
    public decimal CurrentLoadKg { get; set; }
    public string Status { get; set; } = string.Empty;
    public string GpsTrackingStatus { get; set; } = string.Empty;
    public GpsCoordinateDto? LastLocation { get; set; }
    public DateTime? LastLocationUpdatedAt { get; set; }
    public string? RegistrationCertificateUrl { get; set; }
    public string? InsuranceDocumentUrl { get; set; }
    public DateTime? NextServiceDate { get; set; }
}
