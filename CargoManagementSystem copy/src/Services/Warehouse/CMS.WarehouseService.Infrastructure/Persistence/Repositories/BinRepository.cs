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

    /// <summary>
    /// Returns the first available bin with sufficient capacity for the given weight.
    /// </summary>
    public async Task<Bin?> GetAvailableAsync(Guid warehouseId, decimal requiredWeightKg = 0)
        => await _context.Bins
            .Where(b => b.WarehouseId == warehouseId
                        && !b.IsOccupied
                        && b.IsActive
                        && b.CapacityKg >= requiredWeightKg)
            .OrderBy(b => b.BinCode)
            .FirstOrDefaultAsync();

    public async Task<IEnumerable<Bin>> GetAllByWarehouseAsync(
        Guid warehouseId,
        bool? available = null,
        string? zone = null,
        string? level = null)
    {
        var query = _context.Bins
            .Where(b => b.WarehouseId == warehouseId)
            .AsQueryable();

        if (available.HasValue)
            query = available.Value
                ? query.Where(b => !b.IsOccupied && b.IsActive)
                : query.Where(b => b.IsOccupied || !b.IsActive);

        if (!string.IsNullOrWhiteSpace(zone))
            query = query.Where(b => b.Zone.ToLower() == zone.ToLower());

        if (!string.IsNullOrWhiteSpace(level))
            query = query.Where(b => b.Level.ToLower() == level.ToLower());

        return await query.OrderBy(b => b.Zone).ThenBy(b => b.Level).ThenBy(b => b.BinCode).ToListAsync();
    }

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
