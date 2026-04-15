namespace CMS.WarehouseService.Application.DTOs;

public class ReceiveCargoRequest
{
    public Guid ShipmentId { get; set; }
    public Guid WarehouseId { get; set; }
    public bool HasDamageReport { get; set; }
    public string? DamageNotes { get; set; }
}
