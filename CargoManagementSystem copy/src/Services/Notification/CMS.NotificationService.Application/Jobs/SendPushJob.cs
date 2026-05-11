using CMS.NotificationService.Application.Interfaces;
using CMS.NotificationService.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.Jobs;

public class SendPushJob
{
    private readonly IPushService _pushService;
    private readonly INotificationRepository _repository;
    private readonly ILogger<SendPushJob> _logger;

    public SendPushJob(
        IPushService pushService,
        INotificationRepository repository,
        ILogger<SendPushJob> logger)
    {
        _pushService = pushService;
        _repository = repository;
        _logger = logger;
    }

    [AutomaticRetry(Attempts = 5, DelaysInSeconds = new[] { 60, 120, 240, 480, 960 })]
    public async Task Execute(Guid notificationId)
    {
        var record = await _repository.GetByIdAsync(notificationId);
        if (record is null)
        {
            _logger.LogWarning("Notification record {Id} not found", notificationId);
            return;
        }

        record.IncrementRetry();

        var success = await _pushService.SendAsync(record.Recipient, record.Subject, record.Body);

        if (success)
        {
            record.MarkSent();
            _logger.LogInformation("Push notification sent for {Id}", notificationId);
        }
        else
        {
            if (record.RetryCount >= 5)
            {
                record.MarkFailed("Push delivery failed after maximum retries.");
                _logger.LogWarning("Push notification {Id} marked Failed after {RetryCount} attempts", notificationId, record.RetryCount);
            }
            else
            {
                _logger.LogWarning("Push notification {Id} failed, retry {RetryCount}", notificationId, record.RetryCount);
            }
        }

        await _repository.UpdateAsync(record);
    }
}
