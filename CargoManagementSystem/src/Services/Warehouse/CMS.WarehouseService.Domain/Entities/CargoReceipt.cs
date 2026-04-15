using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public class CargoReceipt : BaseEntity
{
    public Guid ShipmentId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public Guid BinId { get; private set; }
    public bool HasDamageReport { get; private set; }
    public string DamageNotes { get; private set; } = string.Empty;
    public string ReceivedByUserId { get; private set; } = string.Empty;
    public DateTime ReceivedAt { get; private set; }

    private CargoReceipt() { }

    public static CargoReceipt Create(
        Guid shipmentId,
        Guid warehouseId,
        Guid binId,
        string receivedByUserId,
        bool hasDamageReport,
        string damageNotes)
    {
        return new CargoReceipt
        {
            ShipmentId = shipmentId,
            WarehouseId = warehouseId,
            BinId = binId,
            ReceivedByUserId = receivedByUserId,
            HasDamageReport = hasDamageReport,
            DamageNotes = damageNotes ?? string.Empty,
            ReceivedAt = DateTime.UtcNow
        };
    }
}
