using System.Text;
using System.Text.Json;
using CMS.ReportingService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.ReportingService.Infrastructure.Services;

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

    public async Task SendReportReadyNotificationAsync(string userId, string reportType, string downloadUrl)
    {
        var payload = new
        {
            type = "ReportReady",
            userId,
            reportType,
            downloadUrl,
            message = $"Your {reportType} is ready for download."
        };

        var content = new StringContent(
            JsonSerializer.Serialize(payload, _jsonOptions),
            Encoding.UTF8,
            "application/json");

        try
        {
            var response = await _httpClient.PostAsync("api/v1/notifications/report-ready", content);
            if (!response.IsSuccessStatusCode)
                _logger.LogWarning("Notification service returned {StatusCode} for user {UserId}", response.StatusCode, userId);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send report-ready notification to user {UserId}", userId);
        }
    }
}
