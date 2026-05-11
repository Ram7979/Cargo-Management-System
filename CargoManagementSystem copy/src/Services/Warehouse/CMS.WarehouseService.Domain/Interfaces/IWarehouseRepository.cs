using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<Warehouse?> GetByIdAsync(Guid id);
    Task<IEnumerable<Warehouse>> GetAllAsync(string? city = null, string? country = null);
    Task AddAsync(Warehouse warehouse);
    Task UpdateAsync(Warehouse warehouse);
}
