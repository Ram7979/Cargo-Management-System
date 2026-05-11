namespace CMS.ShipmentService.Application.DTOs;

public class UpdateShipmentStatusRequest
{
    public string Status { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public string? PodImageUrl { get; set; }
    public string? PodSignatureData { get; set; }

    // GPS at time of update
    public double? GpsLatitude { get; set; }
    public double? GpsLongitude { get; set; }

    // Failed delivery
    public string? FailureReason { get; set; }
    public DateTime? ReDeliveryScheduledAt { get; set; }
}
