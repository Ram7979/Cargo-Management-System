namespace CMS.ReportingService.Application.DTOs;

public class DashboardSummaryDto
{
    public int TotalActiveShipments { get; set; }
    public Dictionary<string, int> ShipmentsByStatus { get; set; } = new();
    public int PendingInvoicesCount { get; set; }
    public int TotalShipmentsToday { get; set; }
    public int DeliveredToday { get; set; }
    public int PendingPickups { get; set; }
    public int InTransitCount { get; set; }
    public int FailedDeliveries { get; set; }
    public decimal RevenueThisMonth { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
}

public class DashboardKpiDto
{
    public string Label { get; set; } = string.Empty;
    public decimal Value { get; set; }
    public string Unit { get; set; } = string.Empty;
    public double? ChangePercent { get; set; }
    public string Trend { get; set; } = string.Empty; // "up", "down", "flat"
}

public class TrendDataDto
{
    public IEnumerable<RevenueTrendPointDto> RevenueTrend { get; set; } = new List<RevenueTrendPointDto>();
    public IEnumerable<StatusTrendPointDto> ShipmentsByStatusOverTime { get; set; } = new List<StatusTrendPointDto>();
    public IEnumerable<CargoTypeBreakdownDto> CargoTypeBreakdown { get; set; } = new List<CargoTypeBreakdownDto>();
    public IEnumerable<GeographicVolumeDto> GeographicVolume { get; set; } = new List<GeographicVolumeDto>();
}

public class RevenueTrendPointDto
{
    public string Period { get; set; } = string.Empty;
    public decimal Revenue { get; set; }
}

public class StatusTrendPointDto
{
    public string Period { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int Count { get; set; }
}

public class CargoTypeBreakdownDto
{
    public string CargoType { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class GeographicVolumeDto
{
    public string City { get; set; } = string.Empty;
    public string Country { get; set; } = string.Empty;
    public int ShipmentCount { get; set; }
}
