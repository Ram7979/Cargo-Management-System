namespace CMS.BillingService.Application.DTOs;

public class GenerateInvoiceRequest
{
    public Guid ShipmentId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal BaseFreightCharge { get; set; }
    public decimal FuelSurcharge { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal TaxRate { get; set; } = 0.18m; // 18% GST default
    public DateTime? DueDate { get; set; }
    public string Notes { get; set; } = string.Empty;
}
