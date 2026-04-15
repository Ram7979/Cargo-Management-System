using CMS.Shared.Events;

namespace CMS.WarehouseService.Application.Events;

public class CargoReceivedEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public Guid WarehouseId { get; }
    public Guid BinId { get; }
    public Guid ReceiptId { get; }

    public CargoReceivedEvent(Guid shipmentId, Guid warehouseId, Guid binId, Guid receiptId)
    {
        ShipmentId = shipmentId;
        WarehouseId = warehouseId;
        BinId = binId;
        ReceiptId = receiptId;
    }
}
