using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface IBinRepository
{
    Task<Bin?> GetByIdAsync(Guid id);
    Task<Bin?> GetAvailableAsync(Guid warehouseId);
    Task<IEnumerable<Bin>> GetAllByWarehouseAsync(Guid warehouseId);
    Task AddAsync(Bin bin);
    Task UpdateAsync(Bin bin);
}
