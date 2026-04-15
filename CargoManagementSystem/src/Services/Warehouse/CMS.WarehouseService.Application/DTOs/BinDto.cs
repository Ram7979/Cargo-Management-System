namespace CMS.WarehouseService.Application.DTOs;

public class BinDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public decimal CapacityKg { get; set; }
}
