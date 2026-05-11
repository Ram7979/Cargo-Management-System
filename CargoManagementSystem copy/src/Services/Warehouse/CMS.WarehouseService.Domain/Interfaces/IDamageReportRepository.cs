using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface IDamageReportRepository
{
    Task<DamageReport?> GetByIdAsync(Guid id);
    Task<IEnumerable<DamageReport>> GetAllAsync(Guid? warehouseId = null, string? status = null, DateTime? dateFrom = null, DateTime? dateTo = null);
    Task AddAsync(DamageReport report);
    Task UpdateAsync(DamageReport report);
}
