using CMS.BillingService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.BillingService.Domain.Entities;

public class Invoice : BaseEntity
{
    public string InvoiceNumber { get; private set; } = string.Empty;
    public Guid ShipmentId { get; private set; }
    public Guid CustomerId { get; private set; }

    // Line items
    public decimal BaseFreightCharge { get; private set; }
    public decimal FuelSurcharge { get; private set; }
    public decimal HandlingFee { get; private set; }
    public decimal InsuranceAmount { get; private set; }
    public decimal TaxRate { get; private set; }
    public decimal TaxAmount { get; private set; }
    public decimal TotalAmount { get; private set; }
    public decimal OutstandingBalance { get; private set; }

    public InvoiceStatus Status { get; private set; }
    public string PdfBlobUrl { get; private set; } = string.Empty;
    public DateTime? DueDate { get; private set; }
    public DateTime InvoiceDate { get; private set; }
    public string Notes { get; private set; } = string.Empty;
    public string? VoidReason { get; private set; }

    private readonly List<Payment> _payments = new();
    public ICollection<Payment> Payments => _payments.AsReadOnly();

    private Invoice() { }

    public static Invoice Create(
        string invoiceNumber,
        Guid shipmentId,
        Guid customerId,
        decimal baseFreightCharge,
        decimal fuelSurcharge,
        decimal handlingFee,
        decimal insuranceAmount,
        decimal taxRate,
        DateTime? dueDate,
        string notes = "")
    {
        var taxAmount = Math.Round((baseFreightCharge + fuelSurcharge + handlingFee + insuranceAmount) * taxRate, 2);
        var total = baseFreightCharge + fuelSurcharge + handlingFee + insuranceAmount + taxAmount;

        return new Invoice
        {
            InvoiceNumber = invoiceNumber,
            ShipmentId = shipmentId,
            CustomerId = customerId,
            BaseFreightCharge = baseFreightCharge,
            FuelSurcharge = fuelSurcharge,
            HandlingFee = handlingFee,
            InsuranceAmount = insuranceAmount,
            TaxRate = taxRate,
            TaxAmount = taxAmount,
            TotalAmount = total,
            OutstandingBalance = total,
            Status = InvoiceStatus.Draft,
            DueDate = dueDate,
            InvoiceDate = DateTime.UtcNow,
            Notes = notes
        };
    }

    public void ApplyPayment(decimal amount)
    {
        OutstandingBalance -= amount;
        Status = OutstandingBalance <= 0 ? InvoiceStatus.Paid : InvoiceStatus.PartiallyPaid;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SetPdfUrl(string url)
    {
        PdfBlobUrl = url;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkIssued()
    {
        Status = InvoiceStatus.Issued;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkOverdue()
    {
        if (Status == InvoiceStatus.Issued || Status == InvoiceStatus.PartiallyPaid)
        {
            Status = InvoiceStatus.Overdue;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public void Void(string reason)
    {
        if (Status == InvoiceStatus.Paid)
            throw new InvalidOperationException("Cannot void a fully paid invoice.");
        Status = InvoiceStatus.Void;
        VoidReason = reason;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkPaid(string referenceNumber)
    {
        OutstandingBalance = 0;
        Status = InvoiceStatus.Paid;
        UpdatedAt = DateTime.UtcNow;
    }

    public bool IsFullyPaid() => OutstandingBalance <= 0;

    public bool CanAcceptPayment() =>
        Status == InvoiceStatus.Issued ||
        Status == InvoiceStatus.PartiallyPaid ||
        Status == InvoiceStatus.Overdue;

    public bool IsOverdue() =>
        DueDate.HasValue &&
        DueDate.Value < DateTime.UtcNow &&
        Status != InvoiceStatus.Paid &&
        Status != InvoiceStatus.Void;
}
