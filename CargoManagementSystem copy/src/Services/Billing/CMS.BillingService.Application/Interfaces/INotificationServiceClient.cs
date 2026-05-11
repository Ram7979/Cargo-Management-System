namespace CMS.BillingService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task SendInvoiceEmailAsync(string recipientId, string email, string invoiceNumber, string pdfUrl);
    Task SendReceiptEmailAsync(string recipientId, string email, string invoiceNumber, string receiptUrl);
}
