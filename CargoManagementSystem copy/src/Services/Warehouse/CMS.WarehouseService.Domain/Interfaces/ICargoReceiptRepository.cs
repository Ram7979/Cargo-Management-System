using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface ICargoReceiptRepository
{
    Task<CargoReceipt?> GetByIdAsync(Guid id);
    Task<CargoReceipt?> GetByShipmentIdAsync(Guid shipmentId);
    Task<(IEnumerable<CargoReceipt> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Guid? warehouseId = null,
        Guid? shipmentId = null,
        bool? hasDamage = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task AddAsync(CargoReceipt receipt);
    Task UpdateAsync(CargoReceipt receipt);
}
