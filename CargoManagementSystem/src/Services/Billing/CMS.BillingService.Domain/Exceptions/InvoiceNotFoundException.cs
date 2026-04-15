namespace CMS.BillingService.Domain.Exceptions;

public class InvoiceNotFoundException : Exception
{
    public InvoiceNotFoundException(Guid invoiceId)
        : base($"Invoice with ID '{invoiceId}' was not found.") { }

    public InvoiceNotFoundException(string message) : base(message) { }
}
