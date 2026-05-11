using CMS.BillingService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.BillingService.Domain.Entities;

public class Payment : BaseEntity
{
    public Guid InvoiceId { get; private set; }
    public decimal Amount { get; private set; }
    public PaymentMethod Method { get; private set; }
    public string ReferenceNumber { get; private set; } = string.Empty;
    public string ReceiptBlobUrl { get; private set; } = string.Empty;
    public DateTime PaidAt { get; private set; }
    public string RecordedBy { get; private set; } = string.Empty;
    public string Notes { get; private set; } = string.Empty;
    public bool IsRefund { get; private set; }
    public string? RefundReason { get; private set; }

    private Payment() { }

    public static Payment Create(
        Guid invoiceId, decimal amount, PaymentMethod method,
        string referenceNumber, string recordedBy = "", string notes = "",
        DateTime? paidAt = null)
    {
        return new Payment
        {
            InvoiceId = invoiceId,
            Amount = amount,
            Method = method,
            ReferenceNumber = referenceNumber,
            RecordedBy = recordedBy,
            Notes = notes,
            PaidAt = paidAt ?? DateTime.UtcNow,
            IsRefund = false
        };
    }

    public static Payment CreateRefund(
        Guid invoiceId, decimal amount, PaymentMethod method,
        string referenceNumber, string reason, string recordedBy = "")
    {
        return new Payment
        {
            InvoiceId = invoiceId,
            Amount = -Math.Abs(amount), // negative amount for refund
            Method = method,
            ReferenceNumber = referenceNumber,
            RecordedBy = recordedBy,
            Notes = reason,
            PaidAt = DateTime.UtcNow,
            IsRefund = true,
            RefundReason = reason
        };
    }

    public void SetReceiptUrl(string url)
    {
        ReceiptBlobUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }
}
