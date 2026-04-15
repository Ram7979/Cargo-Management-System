namespace CMS.ShipmentService.Application.DTOs;

public class RateCalculationDto
{
    public decimal BaseFreightCharge { get; set; }
    public decimal FuelSurcharge { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public string Currency { get; set; } = "USD";
    public string ServiceType { get; set; } = string.Empty;
    public int EstimatedDays { get; set; }
}
