using CMS.Shared.Entities;

namespace CMS.WarehouseService.Domain.Entities;

public class Bin : BaseEntity
{
    public Guid WarehouseId { get; private set; }
    public string BinCode { get; private set; } = string.Empty;
    public string Zone { get; private set; } = string.Empty;
    public string Level { get; private set; } = string.Empty;
    public bool IsOccupied { get; private set; }
    public bool IsActive { get; private set; } = true;
    public decimal CapacityKg { get; private set; }

    private Bin() { }

    public static Bin Create(Guid warehouseId, string binCode, decimal capacityKg,
        string zone = "", string level = "")
    {
        return new Bin
        {
            WarehouseId = warehouseId,
            BinCode = binCode,
            Zone = zone,
            Level = level,
            CapacityKg = capacityKg,
            IsOccupied = false,
            IsActive = true
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

    public void Update(string? binCode, string? zone, string? level, decimal? capacityKg, bool? isActive)
    {
        if (binCode != null) BinCode = binCode;
        if (zone != null) Zone = zone;
        if (level != null) Level = level;
        if (capacityKg.HasValue && !IsOccupied) CapacityKg = capacityKg.Value;
        if (isActive.HasValue) IsActive = isActive.Value;
        UpdatedAt = DateTime.UtcNow;
    }
}
