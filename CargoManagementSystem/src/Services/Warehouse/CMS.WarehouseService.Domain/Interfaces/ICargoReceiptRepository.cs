using CMS.WarehouseService.Domain.Entities;

namespace CMS.WarehouseService.Domain.Interfaces;

public interface ICargoReceiptRepository
{
    Task<CargoReceipt?> GetByIdAsync(Guid id);
    Task<CargoReceipt?> GetByShipmentIdAsync(Guid shipmentId);
    Task AddAsync(CargoReceipt receipt);
}
