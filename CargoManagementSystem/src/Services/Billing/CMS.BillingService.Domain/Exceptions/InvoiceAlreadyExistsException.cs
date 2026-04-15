namespace CMS.BillingService.Domain.Exceptions;

public class InvoiceAlreadyExistsException : Exception
{
    public InvoiceAlreadyExistsException(Guid shipmentId)
        : base($"An invoice already exists for shipment '{shipmentId}'.") { }

    public InvoiceAlreadyExistsException(string message) : base(message) { }
}
