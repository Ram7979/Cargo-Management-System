using CMS.Shared.Entities;
using CMS.ShipmentService.Domain.Enums;
using CMS.Shared.Exceptions;
using CMS.ShipmentService.Domain.StateMachine;
using CMS.ShipmentService.Domain.ValueObjects;

namespace CMS.ShipmentService.Domain.Entities;

public class Shipment : BaseEntity
{
    // Core identifiers
    public string TrackingNumber { get; private set; } = string.Empty;
    public Guid CustomerId { get; private set; }
    public ShipmentStatus Status { get; private set; }

    // Sender details
    public string SenderName { get; private set; } = string.Empty;
    public string OriginAddress { get; private set; } = string.Empty;
    public string SenderCity { get; private set; } = string.Empty;
    public string SenderZip { get; private set; } = string.Empty;
    public string SenderCountry { get; private set; } = string.Empty;
    public string SenderContact { get; private set; } = string.Empty;

    // Recipient details
    public string RecipientName { get; private set; } = string.Empty;
    public string DestinationAddress { get; private set; } = string.Empty;
    public string RecipientCity { get; private set; } = string.Empty;
    public string RecipientZip { get; private set; } = string.Empty;
    public string RecipientCountry { get; private set; } = string.Empty;
    public string RecipientContact { get; private set; } = string.Empty;

    // Cargo details
    public decimal WeightKg { get; private set; }
    public decimal VolumeCbm { get; private set; }
    public int Quantity { get; private set; }
    public CargoType CargoType { get; private set; }
    public decimal DeclaredValue { get; private set; }
    public string CargoDescription { get; private set; } = string.Empty;

    // Service & payment
    public string ServiceType { get; private set; } = string.Empty;
    public PaymentMode PaymentMode { get; private set; }

    // International / DG
    public string? HsCode { get; private set; }
    public string? CountryOfOrigin { get; private set; }
    public bool IsDangerousGoods { get; private set; }

    // Documents
    public string BolDocumentUrl { get; private set; } = string.Empty;
    public string PodImageUrl { get; private set; } = string.Empty;
    public string PodSignatureData { get; private set; } = string.Empty;

    // Tracking
    public GpsCoordinate? LastKnownLocation { get; private set; }
    public DateTime? EstimatedDeliveryDate { get; private set; }

    // Cancellation
    public bool RefundEligible { get; private set; }
    public string? CancellationReason { get; private set; }
    public DateTime? CancelledAt { get; private set; }

    // Failed delivery
    public FailureReason? LastFailureReason { get; private set; }
    public DateTime? ReDeliveryScheduledAt { get; private set; }

    private readonly List<ShipmentStatusHistory> _statusHistory = new();
    public ICollection<ShipmentStatusHistory> StatusHistory => _statusHistory;

    private Shipment() { }

    public static Shipment Create(
        string trackingNumber,
        Guid customerId,
        string senderName, string originAddress, string senderCity, string senderZip, string senderCountry, string senderContact,
        string recipientName, string destinationAddress, string recipientCity, string recipientZip, string recipientCountry, string recipientContact,
        decimal weightKg, decimal volumeCbm, int quantity, CargoType cargoType, decimal declaredValue, string cargoDescription,
        string serviceType, PaymentMode paymentMode,
        string? hsCode = null, string? countryOfOrigin = null, bool isDangerousGoods = false)
    {
        return new Shipment
        {
            TrackingNumber = trackingNumber,
            CustomerId = customerId,
            Status = ShipmentStatus.Pending,
            SenderName = senderName,
            OriginAddress = originAddress,
            SenderCity = senderCity,
            SenderZip = senderZip,
            SenderCountry = senderCountry,
            SenderContact = senderContact,
            RecipientName = recipientName,
            DestinationAddress = destinationAddress,
            RecipientCity = recipientCity,
            RecipientZip = recipientZip,
            RecipientCountry = recipientCountry,
            RecipientContact = recipientContact,
            WeightKg = weightKg,
            VolumeCbm = volumeCbm,
            Quantity = quantity,
            CargoType = cargoType,
            DeclaredValue = declaredValue,
            CargoDescription = cargoDescription,
            ServiceType = serviceType,
            PaymentMode = paymentMode,
            HsCode = hsCode,
            CountryOfOrigin = countryOfOrigin,
            IsDangerousGoods = isDangerousGoods
        };
    }

    public void UpdateStatus(ShipmentStatus newStatus, string actorId, string notes, GpsCoordinate? location = null)
    {
        if (!ShipmentStateMachine.CanTransition(Status, newStatus))
            throw new InvalidStatusTransitionException($"Cannot transition from {Status} to {newStatus}.");

        var history = ShipmentStatusHistory.Create(Id, Status, newStatus, actorId, notes, location);
        _statusHistory.Add(history);

        Status = newStatus;
        UpdatedAt = DateTime.UtcNow;

        if (newStatus == ShipmentStatus.Cancelled)
        {
            RefundEligible = true;
            CancelledAt = DateTime.UtcNow;
        }

        if (location != null)
            LastKnownLocation = location;
    }

    public void SetCancellationReason(string reason) => CancellationReason = reason;

    public void SetFailureReason(FailureReason reason, DateTime? reDeliveryAt = null)
    {
        LastFailureReason = reason;
        ReDeliveryScheduledAt = reDeliveryAt;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPod(string podImageUrl, string podSignatureData)
    {
        PodImageUrl = podImageUrl;
        PodSignatureData = podSignatureData;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetBolUrl(string url)
    {
        BolDocumentUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void UpdateLocation(GpsCoordinate location)
    {
        LastKnownLocation = location;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetEstimatedDeliveryDate(DateTime date)
    {
        EstimatedDeliveryDate = date;
        UpdatedAt = DateTime.UtcNow;
    }
}
