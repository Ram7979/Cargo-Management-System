using CMS.Shared.Events;

namespace CMS.WarehouseService.Application.Events;

public class DamageReportedEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public Guid ReceiptId { get; }
    public string DamageNotes { get; }

    public DamageReportedEvent(Guid shipmentId, Guid receiptId, string damageNotes)
    {
        ShipmentId = shipmentId;
        ReceiptId = receiptId;
        DamageNotes = damageNotes;
    }
}
