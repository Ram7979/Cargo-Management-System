using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.FleetService.Infrastructure.Persistence.Repositories;

public class DriverRepository : IDriverRepository
{
    private readonly FleetDbContext _context;

    public DriverRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<Driver?> GetByIdAsync(Guid id)
        => await _context.Drivers.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<Driver?> GetByUserIdAsync(Guid userId)
        => await _context.Drivers.FirstOrDefaultAsync(d => d.UserId == userId);

    public async Task<(IEnumerable<Driver> Items, int TotalCount)> GetPagedAsync(int page, int pageSize, string? status = null)
    {
        var query = _context.Drivers.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<DriverStatus>(status, ignoreCase: true, out var ds))
            query = query.Where(d => d.Status == ds);

        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(d => d.EmployeeId)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Driver>> GetAllAsync()
        => await _context.Drivers.OrderBy(d => d.EmployeeId).ToListAsync();

    public async Task<IEnumerable<Driver>> GetAvailableAsync()
        => await _context.Drivers.Where(d => d.Status == DriverStatus.Available).ToListAsync();

    public async Task AddAsync(Driver driver)
    {
        await _context.Drivers.AddAsync(driver);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Driver driver)
    {
        _context.Drivers.Update(driver);
        await _context.SaveChangesAsync();
    }
}
