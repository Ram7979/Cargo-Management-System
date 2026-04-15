using CMS.Shared.Events;

namespace CMS.NotificationService.Application.EventHandlers;

public class DamageReportedDomainEvent : IDomainEvent
{
    public Guid ShipmentId { get; }
    public Guid ReceiptId { get; }
    public string DamageNotes { get; }
    public string OpsManagerEmail { get; }

    public DamageReportedDomainEvent(
        Guid shipmentId,
        Guid receiptId,
        string damageNotes,
        string opsManagerEmail)
    {
        ShipmentId = shipmentId;
        ReceiptId = receiptId;
        DamageNotes = damageNotes;
        OpsManagerEmail = opsManagerEmail;
    }
}
