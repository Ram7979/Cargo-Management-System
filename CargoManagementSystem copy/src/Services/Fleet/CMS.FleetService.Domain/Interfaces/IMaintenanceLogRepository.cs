using CMS.FleetService.Domain.Entities;

namespace CMS.FleetService.Domain.Interfaces;

public interface IMaintenanceLogRepository
{
    Task<IEnumerable<MaintenanceLog>> GetByVehicleIdAsync(Guid vehicleId);
    Task AddAsync(MaintenanceLog log);
}
