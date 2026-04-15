namespace CMS.FleetService.Application.DTOs;

public class RegisterDriverRequest
{
    public Guid UserId { get; set; }
    public string EmployeeId { get; set; } = string.Empty;
    public string LicenseNumber { get; set; } = string.Empty;
    public DateTime LicenseExpiry { get; set; }
    public string Phone { get; set; } = string.Empty;
    public DateTime? DateJoined { get; set; }
}
