namespace CMS.CustomerService.Application.DTOs;

public class CustomerDashboardSummaryDto
{
    public int ActiveShipments { get; set; }
    public int DeliveredThisMonth { get; set; }
    public int TotalShipments { get; set; }
    public int PendingInvoicesCount { get; set; }
    public decimal PendingInvoicesAmount { get; set; }
    public int OverdueInvoicesCount { get; set; }
    public List<ShipmentSummaryDto> RecentShipments { get; set; } = new();
    public List<NotificationDto> RecentNotifications { get; set; } = new();
}

public class ShipmentSummaryDto
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string Origin { get; set; } = string.Empty;
    public string Destination { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string ServiceType { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
}

public class NotificationDto
{
    public string Id { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public DateTime CreatedAt { get; set; }
    public bool IsRead { get; set; }
}
