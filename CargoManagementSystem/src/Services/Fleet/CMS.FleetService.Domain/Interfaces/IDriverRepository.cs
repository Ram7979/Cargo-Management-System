using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;

namespace CMS.FleetService.Domain.Interfaces;

public interface IDriverRepository
{
    Task<Driver?> GetByIdAsync(Guid id);
    Task<Driver?> GetByUserIdAsync(Guid userId);
    Task<(IEnumerable<Driver> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null);
    Task<IEnumerable<Driver>> GetAllAsync();
    Task<IEnumerable<Driver>> GetAvailableAsync();
    Task AddAsync(Driver driver);
    Task UpdateAsync(Driver driver);
}
