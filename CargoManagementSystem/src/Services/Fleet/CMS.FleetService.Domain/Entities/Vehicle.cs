using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.ValueObjects;
using CMS.Shared.Entities;

namespace CMS.FleetService.Domain.Entities;

public class Vehicle : BaseEntity
{
    public string PlateNumber { get; private set; } = string.Empty;
    public string GpsDeviceId { get; private set; } = string.Empty;
    public VehicleType Type { get; private set; }
    public string Make { get; private set; } = string.Empty;
    public string Model { get; private set; } = string.Empty;
    public int Year { get; private set; }
    public FuelType FuelType { get; private set; }
    public decimal CapacityKg { get; private set; }
    public decimal VolumeCbm { get; private set; }
    public decimal CurrentLoadKg { get; private set; }
    public VehicleStatus Status { get; private set; }
    public GpsTrackingStatus GpsTrackingStatus { get; private set; }
    public GpsCoordinate? LastLocation { get; private set; }
    public DateTime? LastLocationUpdatedAt { get; private set; }
    public string? RegistrationCertificateUrl { get; private set; }
    public string? InsuranceDocumentUrl { get; private set; }
    public DateTime? NextServiceDate { get; private set; }

    private readonly List<MaintenanceLog> _maintenanceLogs = new();
    public ICollection<MaintenanceLog> MaintenanceLogs => _maintenanceLogs.AsReadOnly();

    private Vehicle() { }

    public static Vehicle Create(
        string plateNumber, string gpsDeviceId, VehicleType type,
        decimal capacityKg, string make = "", string model = "",
        int year = 0, FuelType fuelType = FuelType.Diesel,
        decimal volumeCbm = 0, string? registrationCertUrl = null,
        string? insuranceDocUrl = null, DateTime? nextServiceDate = null)
    {
        var gpsStatus = string.IsNullOrWhiteSpace(gpsDeviceId)
            ? GpsTrackingStatus.NotConfigured
            : GpsTrackingStatus.Active;

        return new Vehicle
        {
            PlateNumber = plateNumber,
            GpsDeviceId = gpsDeviceId,
            Type = type,
            Make = make,
            Model = model,
            Year = year,
            FuelType = fuelType,
            CapacityKg = capacityKg,
            VolumeCbm = volumeCbm,
            CurrentLoadKg = 0,
            Status = VehicleStatus.Available,
            GpsTrackingStatus = gpsStatus,
            RegistrationCertificateUrl = registrationCertUrl,
            InsuranceDocumentUrl = insuranceDocUrl,
            NextServiceDate = nextServiceDate
        };
    }

    public void UpdateDetails(string? make, string? model, int? year, FuelType? fuelType,
        decimal? capacityKg, decimal? volumeCbm, string? gpsDeviceId, string? insuranceDocUrl)
    {
        if (make != null) Make = make;
        if (model != null) Model = model;
        if (year.HasValue) Year = year.Value;
        if (fuelType.HasValue) FuelType = fuelType.Value;
        if (capacityKg.HasValue) CapacityKg = capacityKg.Value;
        if (volumeCbm.HasValue) VolumeCbm = volumeCbm.Value;
        if (gpsDeviceId != null) GpsDeviceId = gpsDeviceId;
        if (insuranceDocUrl != null) InsuranceDocumentUrl = insuranceDocUrl;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(GpsCoordinate location)
    {
        LastLocation = location;
        LastLocationUpdatedAt = DateTime.UtcNow;
        GpsTrackingStatus = GpsTrackingStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetGpsSignalLost()
    {
        GpsTrackingStatus = GpsTrackingStatus.SignalLost;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetStatus(VehicleStatus status)
    {
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }

    public void AddLoad(decimal weightKg)
    {
        CurrentLoadKg += weightKg;
        UpdatedAt = DateTime.UtcNow;
    }

    public void RemoveLoad(decimal weightKg)
    {
        CurrentLoadKg = Math.Max(0, CurrentLoadKg - weightKg);
        UpdatedAt = DateTime.UtcNow;
    }

    public bool HasCapacityFor(decimal weightKg) => CurrentLoadKg + weightKg <= CapacityKg;

    public bool CanBeAssigned() => Status == VehicleStatus.Available;

    public void AddMaintenanceLog(MaintenanceLog log)
    {
        _maintenanceLogs.Add(log);
        if (log.NextServiceDate.HasValue)
            NextServiceDate = log.NextServiceDate;
        UpdatedAt = DateTime.UtcNow;
    }
}
