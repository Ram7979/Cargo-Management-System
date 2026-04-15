namespace CMS.FleetService.Application.DTOs;

public class DriverDto
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiry { get; set; }
    public string Phone { get; set; } = string.Empty;
    public string? ProfilePhotoUrl { get; set; }
    public DateTime DateJoined { get; set; }
    public string Status { get; set; } = string.Empty;
    public bool IsLicenseExpired { get; set; }
}
