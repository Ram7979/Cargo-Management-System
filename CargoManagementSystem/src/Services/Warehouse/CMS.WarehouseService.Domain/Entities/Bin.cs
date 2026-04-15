using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public class Bin : BaseEntity
{
    public Guid WarehouseId { get; private set; }
    public string BinCode { get; private set; } = string.Empty;
    public bool IsOccupied { get; private set; }
    public decimal CapacityKg { get; private set; }

    private Bin() { }

    public static Bin Create(Guid warehouseId, string binCode, decimal capacityKg)
    {
        return new Bin
        {
            WarehouseId = warehouseId,
            BinCode = binCode,
            CapacityKg = capacityKg,
            IsOccupied = false
        };
    }

    public void Occupy()
    {
        IsOccupied = true;
        UpdatedAt = DateTime.UtcNow;
    }

    public void Release()
    {
        IsOccupied = false;
        UpdatedAt = DateTime.UtcNow;
    }
}
