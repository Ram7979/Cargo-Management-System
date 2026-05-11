namespace CMS.BillingService.Application.DTOs;

public class InvoiceDto
{
    public Guid Id { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public Guid ShipmentId { get; set; }
    public string CustomerId { get; set; } = string.Empty;
    public decimal BaseFreightCharge { get; set; }
    public decimal FuelSurcharge { get; set; }
    public decimal HandlingFee { get; set; }
    public decimal InsuranceAmount { get; set; }
    public decimal TaxRate { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal OutstandingBalance { get; set; }
    public string Status { get; set; } = string.Empty;
    public string PdfBlobUrl { get; set; } = string.Empty;
    public DateTime? DueDate { get; set; }
    public DateTime InvoiceDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public string? VoidReason { get; set; }
    public DateTime CreatedAt { get; set; }
}
