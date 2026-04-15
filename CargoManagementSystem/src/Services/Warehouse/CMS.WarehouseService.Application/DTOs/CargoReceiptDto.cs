namespace CMS.WarehouseService.Application.DTOs;

public class CargoReceiptDto
{
    public Guid Id { get; set; }
    public Guid ShipmentId { get; set; }
    public Guid WarehouseId { get; set; }
    public Guid BinId { get; set; }
    public bool HasDamageReport { get; set; }
    public string DamageNotes { get; set; } = string.Empty;
    public string ReceivedByUserId { get; set; } = string.Empty;
    public DateTime ReceivedAt { get; set; }
}
