using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;

namespace CMS.FleetService.Domain.Interfaces;

public interface IVehicleRepository
{
    Task<Vehicle?> GetByIdAsync(Guid id);
    Task<Vehicle?> GetByPlateNumberAsync(string plateNumber);
    Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null, string? type = null);
    Task<IEnumerable<Vehicle>> GetAllAsync();
    Task<IEnumerable<Vehicle>> GetAvailableAsync(decimal? minCapacityKg = null, string? type = null);
    Task AddAsync(Vehicle vehicle);
    Task UpdateAsync(Vehicle vehicle);
}
