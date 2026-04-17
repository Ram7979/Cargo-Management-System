namespace CMS.ReportingService.Application.DTOs;

public class ShipmentReportDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string CargoType { get; set; } = string.Empty;
    public string? DriverName { get; set; }
    public string? PlateNumber { get; set; }
    public decimal InvoiceAmount { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
}

public class RevenueReportDto
{
    public decimal TotalInvoiced { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal Outstanding { get; set; }
    public IEnumerable<RevenueBreakdownDto> ByServiceType { get; set; } = new List<RevenueBreakdownDto>();
    public IEnumerable<RevenueBreakdownDto> ByCustomer { get; set; } = new List<RevenueBreakdownDto>();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class RevenueBreakdownDto
{
    public string Label { get; set; } = string.Empty;
    public decimal TotalInvoiced { get; set; }
    public decimal TotalCollected { get; set; }
    public decimal Outstanding { get; set; }
}

public class FleetReportDto
{
    public IEnumerable<DriverStatsDto> DriverStats { get; set; } = new List<DriverStatsDto>();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class DriverStatsDto
{
    public string DriverId { get; set; } = string.Empty;
    public string DriverName { get; set; } = string.Empty;
    public int TotalDeliveries { get; set; }
    public int OnTimeDeliveries { get; set; }
    public int FailedDeliveries { get; set; }
    public double OnTimeRatePercent { get; set; }
    public double FailedRatePercent { get; set; }
}

public class CustomerStatementDto
{
    public string CustomerId { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public decimal TotalInvoiced { get; set; }
    public decimal TotalPaid { get; set; }
    public decimal OutstandingBalance { get; set; }
    public int TotalShipments { get; set; }
    public IEnumerable<ShipmentReportDto> Shipments { get; set; } = new List<ShipmentReportDto>();
    public DateTime FromDate { get; set; }
    public DateTime ToDate { get; set; }
}

public class AsyncExportRequestDto
{
    public string ReportType { get; set; } = string.Empty; // "shipments", "revenue", "fleet"
    public ShipmentReportFilter? Filters { get; set; }
    public string Format { get; set; } = "excel"; // "excel" or "pdf"
}

public class AsyncExportStatusDto
{
    public string JobId { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty; // "Queued", "Processing", "Completed", "Failed"
    public string? DownloadUrl { get; set; }
    public DateTime? CompletedAt { get; set; }
}
