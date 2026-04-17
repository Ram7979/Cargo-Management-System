namespace CMS.WarehouseService.Application.DTOs;

public class WarehouseDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal CapacityKg { get; set; }
    public int TotalBins { get; set; }
    public int OccupiedBins { get; set; }
    public int AvailableBins { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class CreateWarehouseRequest
{
    public string Name { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public decimal CapacityKg { get; set; }
    public int TotalBins { get; set; }
}

public class UpdateWarehouseRequest
{
    public string? Name { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? Country { get; set; }
    public decimal? CapacityKg { get; set; }
}

public class InventorySummaryDto
{
    public Guid WarehouseId { get; set; }
    public string WarehouseName { get; set; } = string.Empty;
    public int TotalBins { get; set; }
    public int OccupiedBins { get; set; }
    public int AvailableBins { get; set; }
    public decimal TotalCapacityKg { get; set; }
    public double OccupancyPercent { get; set; }
    public IEnumerable<CurrentShipmentDto> CurrentShipments { get; set; } = new List<CurrentShipmentDto>();
}

public class CurrentShipmentDto
{
    public Guid ShipmentId { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public string BinCode { get; set; } = string.Empty;
    public DateTime ArrivedAt { get; set; }
}
