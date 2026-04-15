using CMS.FleetService.Domain.Entities;

namespace CMS.FleetService.Domain.Interfaces;

public interface IAssignmentRepository
{
    Task<Assignment?> GetByIdAsync(Guid id);
    Task<Assignment?> GetActiveByDriverIdAsync(Guid driverId);
    Task<Assignment?> GetActiveByVehicleIdAsync(Guid vehicleId);
    Task<Assignment?> GetByShipmentIdAsync(Guid shipmentId);
    Task<(IEnumerable<Assignment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        Guid? driverId = null,
        Guid? vehicleId = null,
        Guid? shipmentId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task AddAsync(Assignment assignment);
    Task UpdateAsync(Assignment assignment);
}
