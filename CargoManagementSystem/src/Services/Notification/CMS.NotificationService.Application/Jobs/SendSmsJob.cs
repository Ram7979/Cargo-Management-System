using CMS.NotificationService.Application.Interfaces;
using CMS.NotificationService.Domain.Interfaces;
using Hangfire;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.Jobs;

public class SendSmsJob
{
    private readonly ISmsService _smsService;
    private readonly INotificationRepository _repository;
    private readonly ILogger<SendSmsJob> _logger;

    public SendSmsJob(
        ISmsService smsService,
        INotificationRepository repository,
        ILogger<SendSmsJob> logger)
    {
        _smsService = smsService;
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

        var success = await _smsService.SendAsync(record.Recipient, record.Body);

        if (success)
        {
            record.MarkSent();
            _logger.LogInformation("SMS sent for notification {Id}", notificationId);
        }
        else
        {
            if (record.RetryCount >= 5)
            {
                record.MarkFailed("SMS delivery failed after maximum retries.");
                _logger.LogWarning("SMS notification {Id} marked Failed after {RetryCount} attempts", notificationId, record.RetryCount);
            }
            else
            {
                _logger.LogWarning("SMS notification {Id} failed, retry {RetryCount}", notificationId, record.RetryCount);
            }
        }

        await _repository.UpdateAsync(record);
    }
}
