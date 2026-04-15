namespace CMS.WarehouseService.Domain.Interfaces;

public interface IWarehouseRepository
{
    Task<Entities.Warehouse?> GetByIdAsync(Guid id);
    Task<IEnumerable<Entities.Warehouse>> GetAllAsync();
    Task AddAsync(Entities.Warehouse warehouse);
    Task UpdateAsync(Entities.Warehouse warehouse);
}
