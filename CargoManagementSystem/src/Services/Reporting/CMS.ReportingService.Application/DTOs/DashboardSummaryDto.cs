namespace CMS.ReportingService.Application.DTOs;

public class DashboardSummaryDto
{
    public int TotalActiveShipments { get; set; }
    public Dictionary<string, int> ShipmentsByStatus { get; set; } = new();
    public int PendingInvoicesCount { get; set; }
    public int TotalShipmentsToday { get; set; }
}
