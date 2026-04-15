using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.WarehouseService.Infrastructure.Persistence.Repositories;

public class BinRepository : IBinRepository
{
    private readonly WarehouseDbContext _context;

    public BinRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<Bin?> GetByIdAsync(Guid id)
        => await _context.Bins.FirstOrDefaultAsync(b => b.Id == id);

    public async Task<Bin?> GetAvailableAsync(Guid warehouseId)
        => await _context.Bins
            .Where(b => b.WarehouseId == warehouseId && !b.IsOccupied)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Bin>> GetAllByWarehouseAsync(Guid warehouseId)
        => await _context.Bins
            .Where(b => b.WarehouseId == warehouseId)
            .OrderBy(b => b.BinCode)
            .ToListAsync();

    public async Task AddAsync(Bin bin)
    {
        await _context.Bins.AddAsync(bin);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Bin bin)
    {
        _context.Bins.Update(bin);
        await _context.SaveChangesAsync();
    }
}
