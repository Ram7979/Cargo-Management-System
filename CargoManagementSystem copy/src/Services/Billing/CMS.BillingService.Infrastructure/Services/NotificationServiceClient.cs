using System.Text;
using System.Text.Json;
using CMS.BillingService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.BillingService.Infrastructure.Services;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendInvoiceEmailAsync(string recipientId, string email, string invoiceNumber, string pdfUrl)
    {
        await SendNotificationAsync(recipientId, email,
            $"Invoice {invoiceNumber} — Payment Due",
            $"Your invoice {invoiceNumber} has been generated. PDF: {pdfUrl}",
            "InvoiceGenerated");
    }

    public async Task SendReceiptEmailAsync(string recipientId, string email, string invoiceNumber, string receiptUrl)
    {
        await SendNotificationAsync(recipientId, email,
            $"Payment Receipt — Invoice {invoiceNumber}",
            $"Your payment for invoice {invoiceNumber} has been recorded. Receipt: {receiptUrl}",
            "PaymentRecorded");
    }

    private async Task SendNotificationAsync(string recipientId, string email, string subject, string body, string eventType)
    {
        try
        {
            var payload = new { recipientId, channel = "Email", recipient = email, subject, body, eventType };
            var json = JsonSerializer.Serialize(payload);
            await _httpClient.PostAsync("api/v1/notifications", new StringContent(json, Encoding.UTF8, "application/json"));
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send notification for event {EventType}", eventType);
        }
    }
}
