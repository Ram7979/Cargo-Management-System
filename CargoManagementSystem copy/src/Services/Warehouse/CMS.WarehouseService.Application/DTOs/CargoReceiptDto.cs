namespace CMS.WarehouseService.Application.DTOs;

public class CargoReceiptDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public Guid WarehouseId { get; set; }
    public Guid BinId { get; set; }
    public bool HasDamageReport { get; set; }
    public string DamageNotes { get; set; } = string.Empty;
    public string Remarks { get; set; } = string.Empty;
    public string ReceivedByUserId { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
    public DateTime? ReleasedAt { get; set; }
}

public class ReleaseCargoRequest
{
    public Guid ShipmentId { get; set; }
    public Guid WarehouseId { get; set; }
    public string? Notes { get; set; }
}

public class DamageReportDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid ReceiptId { get; set; }
    public Guid WarehouseId { get; set; }
    public string ReportedByUserId { get; set; } = string.Empty;
    public string DamageNotes { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string? ResolutionNotes { get; set; }
    public DateTime ReportedAt { get; set; }
    public DateTime? ResolvedAt { get; set; }
}

public class UpdateDamageReportRequest
{
    public string Status { get; set; } = string.Empty;
    public string? ResolutionNotes { get; set; }
}

public class ShipmentLookupDto
{
    public Guid ShipmentId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string SenderName { get; set; } = string.Empty;
    public string RecipientName { get; set; } = string.Empty;
    public string CargoDescription { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public decimal VolumeCbm { get; set; }
    public string Status { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
}
