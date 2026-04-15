namespace CMS.ShipmentService.Application.DTOs;

public class CreateShipmentRequest
{
    public Guid CustomerId { get; set; }

    // Sender
    public string SenderName { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string SenderCity { get; set; } = string.Empty;
    public string SenderZip { get; set; } = string.Empty;
    public string SenderCountry { get; set; } = string.Empty;
    public string SenderContact { get; set; } = string.Empty;

    // Recipient
    public string RecipientName { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string RecipientCity { get; set; } = string.Empty;
    public string RecipientZip { get; set; } = string.Empty;
    public string RecipientCountry { get; set; } = string.Empty;
    public string RecipientContact { get; set; } = string.Empty;

    // Cargo
    public decimal WeightKg { get; set; }
    public decimal VolumeCbm { get; set; }
    public int Quantity { get; set; } = 1;
    public string CargoType { get; set; } = "Standard";
    public decimal DeclaredValue { get; set; }
    public string CargoDescription { get; set; } = string.Empty;

    // Service
    public string ServiceType { get; set; } = string.Empty;
    public string PaymentMode { get; set; } = "Prepaid";

    // International / DG (optional)
    public string? HsCode { get; set; }
    public string? CountryOfOrigin { get; set; }
}
