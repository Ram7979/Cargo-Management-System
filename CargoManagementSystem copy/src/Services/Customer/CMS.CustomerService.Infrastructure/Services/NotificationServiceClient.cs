using System.Text;
using System.Text.Json;
using CMS.CustomerService.Application.Interfaces;
using Microsoft.Extensions.Logging;

namespace CMS.CustomerService.Infrastructure.Services;

public class NotificationServiceClient : INotificationServiceClient
{
    private readonly HttpClient _httpClient;
    private readonly ILogger<NotificationServiceClient> _logger;

    public NotificationServiceClient(HttpClient httpClient, ILogger<NotificationServiceClient> logger)
    {
        _httpClient = httpClient;
        _logger = logger;
    }

    public async Task SendWelcomeEmailAsync(string recipientId, string email, string fullName)
    {
        try
        {
            var payload = new
            {
                recipientId,
                channel = "Email",
                recipient = email,
                subject = "Welcome to CMS — Your Account is Ready",
                body = $"Dear {fullName},\n\nWelcome to the Cargo Management System. Your account has been created successfully.\n\nBest regards,\nCMS Team",
                eventType = "CustomerRegistered"
            };

            var json = JsonSerializer.Serialize(payload);
            var content = new StringContent(json, Encoding.UTF8, "application/json");
            await _httpClient.PostAsync("api/v1/notifications", content);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send welcome email to {Email}", email);
        }
    }
}
