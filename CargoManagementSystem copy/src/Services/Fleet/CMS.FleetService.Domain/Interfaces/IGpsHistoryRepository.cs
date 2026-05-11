using CMS.FleetService.Domain.Entities;

namespace CMS.FleetService.Domain.Interfaces;

public interface IGpsHistoryRepository
{
    Task<IEnumerable<GpsHistory>> GetByVehicleIdAsync(Guid vehicleId, DateTime? from = null, DateTime? to = null);
    Task<IEnumerable<GpsHistory>> GetByAssignmentIdAsync(Guid assignmentId);
    Task AddAsync(GpsHistory history);
}
