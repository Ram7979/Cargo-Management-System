using CMS.Shared.Entities;

namespace CMS.ReportingService.Domain.ReadModels;

public class ShipmentReadModel : BaseEntity
{
    public string TrackingNumber { get; set; } = string.Empty;
    public string CustomerCode { get; set; } = string.Empty;
    public string CustomerName { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public string OriginAddress { get; set; } = string.Empty;
    public string OriginCity { get; set; } = string.Empty;
    public string OriginCountry { get; set; } = string.Empty;
    public string DestinationAddress { get; set; } = string.Empty;
    public string DestinationCity { get; set; } = string.Empty;
    public string DestinationCountry { get; set; } = string.Empty;
    public decimal WeightKg { get; set; }
    public string ServiceType { get; set; } = string.Empty;
    public string CargoType { get; set; } = string.Empty;
    public string? DriverId { get; set; }
    public string? DriverName { get; set; }
    public string? VehicleId { get; set; }
    public string? PlateNumber { get; set; }
    public decimal InvoiceAmount { get; set; }
    public new DateTime CreatedAt { get; set; }
    public DateTime? DeliveredAt { get; set; }
    public DateTime? PickedUpAt { get; set; }
}
