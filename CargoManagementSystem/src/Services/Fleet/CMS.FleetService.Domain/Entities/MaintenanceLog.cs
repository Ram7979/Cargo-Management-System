using CMS.Shared.Entities;

namespace CMS.FleetService.Domain.Entities;

public class MaintenanceLog : BaseEntity
{
    public Guid VehicleId { get; private set; }
    public string MaintenanceType { get; private set; } = string.Empty;
    public DateTime ServiceDate { get; private set; }
    public decimal MileageAtService { get; private set; }
    public DateTime? NextServiceDate { get; private set; }
    public decimal? NextServiceMileage { get; private set; }
    public string ServiceProvider { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public decimal Cost { get; private set; }

    private MaintenanceLog() { }

    public static MaintenanceLog Create(
        Guid vehicleId, string maintenanceType, DateTime serviceDate,
        decimal mileageAtService, DateTime? nextServiceDate, decimal? nextServiceMileage,
        string serviceProvider, string notes, decimal cost)
    {
        return new MaintenanceLog
        {
            VehicleId = vehicleId,
            MaintenanceType = maintenanceType,
            ServiceDate = serviceDate,
            MileageAtService = mileageAtService,
            NextServiceDate = nextServiceDate,
            NextServiceMileage = nextServiceMileage,
            ServiceProvider = serviceProvider,
            Notes = notes,
            Cost = cost
        };
    }
}
