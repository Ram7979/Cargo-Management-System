using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public enum DamageReportStatus { Open, Acknowledged, Resolved }

public class DamageReport : BaseEntity
{
    public Guid ShipmentId { get; private set; }
    public Guid ReceiptId { get; private set; }
    public Guid WarehouseId { get; private set; }
    public string ReportedByUserId { get; private set; } = string.Empty;
    public string DamageNotes { get; private set; } = string.Empty;
    public DamageReportStatus Status { get; private set; }
    public string? ResolutionNotes { get; private set; }
    public DateTime ReportedAt { get; private set; }
    public DateTime? ResolvedAt { get; private set; }

    private DamageReport() { }

    public static DamageReport Create(Guid shipmentId, Guid receiptId, Guid warehouseId,
        string reportedByUserId, string damageNotes)
    {
        return new DamageReport
        {
            ShipmentId = shipmentId,
            ReceiptId = receiptId,
            WarehouseId = warehouseId,
            ReportedByUserId = reportedByUserId,
            DamageNotes = damageNotes,
            Status = DamageReportStatus.Open,
            ReportedAt = DateTime.UtcNow
        };
    }

    public void Acknowledge()
    {
        Status = DamageReportStatus.Acknowledged;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Resolve(string resolutionNotes)
    {
        Status = DamageReportStatus.Resolved;
        ResolutionNotes = resolutionNotes;
        ResolvedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
