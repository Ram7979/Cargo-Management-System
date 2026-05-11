using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface IBinRepository
{
    Task<Bin?> GetByIdAsync(Guid id);
    Task<Bin?> GetAvailableAsync(Guid warehouseId, decimal requiredWeightKg = 0);
    Task<IEnumerable<Bin>> GetAllByWarehouseAsync(Guid warehouseId, bool? available = null, string? zone = null, string? level = null);
    Task AddAsync(Bin bin);
    Task UpdateAsync(Bin bin);
}
