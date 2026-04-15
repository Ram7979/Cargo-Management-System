namespace CMS.BillingService.Application.DTOs;

public class PaymentDto
{
    public Guid Id { get; set; }
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public string ReceiptBlobUrl { get; set; } = string.Empty;
    public DateTime PaidAt { get; set; }
    public string RecordedBy { get; set; } = string.Empty;
    public string Notes { get; set; } = string.Empty;
    public bool IsRefund { get; set; }
    public string? RefundReason { get; set; }
}
