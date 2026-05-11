using System.Text;
using System.Text.Json;
using CMS.WarehouseService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Infrastructure.Services;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;
    private static readonly JsonSerializerOptions _jsonOptions = new(JsonSerializerDefaults.Web);

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendWarehouseArrivalNotificationAsync(
        string customerId,
        Guid shipmentId,
        string trackingNumber,
        string warehouseName)
    {
        var payload = new
        {
            type = "WarehouseArrival",
            customerId,
            shipmentId,
            trackingNumber,
            warehouseName,
            message = $"Your shipment {trackingNumber} has arrived at {warehouseName}."
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync("/api/v1/notifications/warehouse-arrival", content);
            if (!response.IsSuccessStatusCode)
            {
                _logger.LogWarning(
                    "Notification service returned {StatusCode} for shipment {TrackingNumber}",
                    response.StatusCode, trackingNumber);
            }
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex,
                "Failed to send warehouse arrival notification for shipment {TrackingNumber}", trackingNumber);
        }
    }
}
