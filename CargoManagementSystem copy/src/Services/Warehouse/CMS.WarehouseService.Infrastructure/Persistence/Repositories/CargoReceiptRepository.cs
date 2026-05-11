using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.WarehouseService.Infrastructure.Persistence.Repositories;

public class CargoReceiptRepository : ICargoReceiptRepository
{
    private readonly WarehouseDbContext _context;

    public CargoReceiptRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<CargoReceipt?> GetByIdAsync(Guid id)
        => await _context.CargoReceipts.FirstOrDefaultAsync(r => r.Id == id);

    public async Task<CargoReceipt?> GetByShipmentIdAsync(Guid shipmentId)
        => await _context.CargoReceipts.FirstOrDefaultAsync(r => r.ShipmentId == shipmentId);

    public async Task<(IEnumerable<CargoReceipt> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        Guid? warehouseId = null,
        Guid? shipmentId = null,
        bool? hasDamage = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.CargoReceipts.AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(r => r.WarehouseId == warehouseId.Value);

        if (shipmentId.HasValue)
            query = query.Where(r => r.ShipmentId == shipmentId.Value);

        if (hasDamage.HasValue)
            query = query.Where(r => r.HasDamageReport == hasDamage.Value);

        if (dateFrom.HasValue)
            query = query.Where(r => r.ReceivedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(r => r.ReceivedAt <= dateTo.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(r => r.ReceivedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(CargoReceipt receipt)
    {
        await _context.CargoReceipts.AddAsync(receipt);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(CargoReceipt receipt)
    {
        _context.CargoReceipts.Update(receipt);
        await _context.SaveChangesAsync();
    }
}
