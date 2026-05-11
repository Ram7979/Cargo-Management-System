namespace CMS.BillingService.Application.DTOs;

public class RecordPaymentRequest
{
    public Guid InvoiceId { get; set; }
    public decimal Amount { get; set; }
    public string Method { get; set; } = string.Empty;
    public string ReferenceNumber { get; set; } = string.Empty;
    public DateTime? PaymentDate { get; set; }
    public string Notes { get; set; } = string.Empty;
    public bool ForceRecord { get; set; } = false;
}
