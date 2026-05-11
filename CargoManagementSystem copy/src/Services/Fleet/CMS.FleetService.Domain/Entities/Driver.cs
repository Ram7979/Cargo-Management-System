using CMS.FleetService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.FleetService.Domain.Entities;

public class Driver : BaseEntity
{
    public Guid UserId { get; private set; }
    public string EmployeeId { get; private set; } = string.Empty;
    public string LicenseNumber { get; private set; } = string.Empty;
    public DateTime LicenseExpiry { get; private set; }
    public string Phone { get; private set; } = string.Empty;
    public string? ProfilePhotoUrl { get; private set; }
    public DateTime DateJoined { get; private set; }
    public DriverStatus Status { get; private set; }

    private Driver() { }

    public static Driver Create(Guid userId, string employeeId, string licenseNumber,
        DateTime licenseExpiry, string phone = "", DateTime? dateJoined = null)
    {
        return new Driver
        {
            UserId = userId,
            EmployeeId = employeeId,
            LicenseNumber = licenseNumber,
            LicenseExpiry = licenseExpiry,
            Phone = phone,
            DateJoined = dateJoined ?? DateTime.UtcNow,
            Status = DriverStatus.Available
        };
    }

    public void SetStatus(DriverStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsLicenseExpired() => LicenseExpiry < DateTime.UtcNow;

    public bool CanBeAssigned() => Status == DriverStatus.Available && !IsLicenseExpired();
}
