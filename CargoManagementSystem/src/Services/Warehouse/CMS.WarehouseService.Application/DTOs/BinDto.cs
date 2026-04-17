namespace CMS.WarehouseService.Application.DTOs;

public class BinDto
{
    public Guid Id { get; set; }
    public Guid WarehouseId { get; set; }
    public string BinCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public bool IsOccupied { get; set; }
    public bool IsActive { get; set; }
    public decimal CapacityKg { get; set; }
}

public class AddBinRequest
{
    public string BinCode { get; set; } = string.Empty;
    public string Zone { get; set; } = string.Empty;
    public string Level { get; set; } = string.Empty;
    public decimal CapacityKg { get; set; }
}

public class UpdateBinRequest
{
    public string? BinCode { get; set; }
    public string? Zone { get; set; }
    public string? Level { get; set; }
    public decimal? CapacityKg { get; set; }
    public bool? IsActive { get; set; }
}
