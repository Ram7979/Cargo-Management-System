using CMS.WarehouseService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.WarehouseService.Infrastructure.Persistence.Repositories;

public class WarehouseRepository : IWarehouseRepository
{
    private readonly WarehouseDbContext _context;

    public WarehouseRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<Domain.Entities.Warehouse?> GetByIdAsync(Guid id)
        => await _context.Warehouses
            .Include(w => w.Bins)
            .Include(w => w.Receipts)
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<IEnumerable<Domain.Entities.Warehouse>> GetAllAsync(string? city = null, string? country = null)
    {
        var query = _context.Warehouses
            .Include(w => w.Bins)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(city))
            query = query.Where(w => w.City.ToLower().Contains(city.ToLower()));

        if (!string.IsNullOrWhiteSpace(country))
            query = query.Where(w => w.Country.ToLower().Contains(country.ToLower()));

        return await query.OrderBy(w => w.Name).ToListAsync();
    }

    public async Task AddAsync(Domain.Entities.Warehouse warehouse)
    {
        await _context.Warehouses.AddAsync(warehouse);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Domain.Entities.Warehouse warehouse)
    {
        _context.Warehouses.Update(warehouse);
        await _context.SaveChangesAsync();
    }
}
