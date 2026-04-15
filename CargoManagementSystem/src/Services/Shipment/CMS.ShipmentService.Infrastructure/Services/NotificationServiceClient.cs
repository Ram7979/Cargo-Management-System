using System.Text;
using System.Text.Json;
using CMS.ShipmentService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.ShipmentService.Infrastructure.Services;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task QueueNotificationAsync(
        string recipientId,
        string channel,
        string recipient,
        string subject,
        string body,
        string eventType)
    {
        var payload = new
        {
            recipientId,
            channel,
            recipient,
            subject,
            body,
            eventType
        };

        var content = new StringContent(JsonSerializer.Serialize(payload), Encoding.UTF8, "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/v1/notifications", content);
            response.EnsureSuccessStatusCode();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue notification via Notification Service for recipient {RecipientId}", recipientId);
            throw;
        }
    }
}
