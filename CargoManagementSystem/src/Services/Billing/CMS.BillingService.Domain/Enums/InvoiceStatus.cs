namespace CMS.BillingService.Domain.Enums;

public enum InvoiceStatus
{
    Draft,
    Issued,
    PartiallyPaid,
    Paid,
    Void,
    Overdue
}
