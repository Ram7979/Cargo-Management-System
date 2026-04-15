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
            .FirstOrDefaultAsync(w => w.Id == id);

    public async Task<IEnumerable<Domain.Entities.Warehouse>> GetAllAsync()
        => await _context.Warehouses
            .Include(w => w.Bins)
            .OrderBy(w => w.Name)
            .ToListAsync();

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
