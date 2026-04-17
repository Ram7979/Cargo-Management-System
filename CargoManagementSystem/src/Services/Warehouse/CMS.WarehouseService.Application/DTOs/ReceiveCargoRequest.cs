namespace CMS.WarehouseService.Application.DTOs;

public class ReceiveCargoRequest
{
    /// <summary>Tracking number from barcode scanner (e.g. CMS-2025-001234)</summary>
    public string TrackingNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid? BinId { get; set; }
    public bool HasDamageReport { get; set; }
    public string? DamageNotes { get; set; }
    public string? Remarks { get; set; }
}
