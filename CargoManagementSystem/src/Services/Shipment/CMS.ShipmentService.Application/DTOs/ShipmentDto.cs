namespace CMS.ShipmentService.Application.DTOs;

public class ShipmentDto
{
    public Guid Id { get; set; }
    public string TrackingNumber { get; set; } = string.Empty;
    public Guid CustomerId { get; set; }
    public string Status { get; set; } = string.Empty;

    // Sender
    public string SenderName { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public string SenderCountry { get; set; } = string.Empty;

    // Recipient
    public string RecipientName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string RecipientCity { get; set; } = string.Empty;
    public string RecipientCountry { get; set; } = string.Empty;

    // Cargo
    public decimal WeightKg { get; set; }
    public decimal VolumeCbm { get; set; }
    public int Quantity { get; set; }
    public string CargoType { get; set; } = string.Empty;
    public decimal DeclaredValue { get; set; }
    public string CargoDescription { get; set; } = string.Empty;

    // Service
    public string ServiceType { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = string.Empty;

    // Documents
    public string BolDocumentUrl { get; set; } = string.Empty;
    public string? PodImageUrl { get; set; }

    // Tracking
    public GpsCoordinateDto? LastKnownLocation { get; set; }
    public DateTime? EstimatedDeliveryDate { get; set; }

    // Cancellation
    public bool RefundEligible { get; set; }
    public string? CancellationReason { get; set; }
    public DateTime? CancelledAt { get; set; }

    // Failed delivery
    public string? LastFailureReason { get; set; }
    public DateTime? ReDeliveryScheduledAt { get; set; }

    public DateTime CreatedAt { get; set; }
}
