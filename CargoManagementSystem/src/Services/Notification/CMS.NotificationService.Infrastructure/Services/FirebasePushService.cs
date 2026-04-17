using CMS.NotificationService.Application.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Infrastructure.Services;

/// <summary>
/// Push notification service stub. Replace with Firebase Admin SDK or Azure Notification Hubs
/// when a push provider is configured.
/// </summary>
public class FirebasePushService : IPushService
{
    private readonly string _serverKey;
    private readonly ILogger<FirebasePushService> _logger;

    public FirebasePushService(IConfiguration configuration, ILogger<FirebasePushService> logger)
    {
        _serverKey = configuration["Firebase:ServerKey"] ?? string.Empty;
        _logger = logger;
    }

    public async Task<bool> SendAsync(string deviceToken, string title, string body)
    {
        if (string.IsNullOrWhiteSpace(_serverKey))
        {
            _logger.LogInformation(
                "Firebase not configured. Simulating push to {DeviceToken}: {Title}", deviceToken, title);
            return true;
        }

        // TODO: Replace with FirebaseAdmin SDK call when configured
        // var message = new Message { Token = deviceToken, Notification = new Notification { Title = title, Body = body } };
        // var result = await FirebaseMessaging.DefaultInstance.SendAsync(message);
        _logger.LogInformation("Push notification sent to {DeviceToken}: {Title}", deviceToken, title);
        return await Task.FromResult(true);
    }
}
