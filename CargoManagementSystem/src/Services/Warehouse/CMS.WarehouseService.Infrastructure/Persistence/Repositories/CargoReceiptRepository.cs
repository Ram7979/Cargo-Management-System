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

    public async Task AddAsync(CargoReceipt receipt)
    {
        await _context.CargoReceipts.AddAsync(receipt);
        await _context.SaveChangesAsync();
    }
}
