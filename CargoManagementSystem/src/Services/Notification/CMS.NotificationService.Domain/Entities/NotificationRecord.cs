using CMS.NotificationService.Domain.Enums;
using CMS.Shared.Entities;

namespace CMS.NotificationService.Domain.Entities;

public class NotificationRecord : BaseEntity
{
    public string RecipientId { get; private set; } = string.Empty;
    public NotificationChannel Channel { get; private set; }
    public string Recipient { get; private set; } = string.Empty;
    public string Subject { get; private set; } = string.Empty;
    public string Body { get; private set; } = string.Empty;
    public NotificationStatus Status { get; private set; }
    public int RetryCount { get; private set; }
    public DateTime? LastAttemptAt { get; private set; }
    public string EventType { get; private set; } = string.Empty;

    private NotificationRecord() { }

    public static NotificationRecord Create(
        string recipientId,
        NotificationChannel channel,
        string recipient,
        string subject,
        string body,
        string eventType)
    {
        return new NotificationRecord
        {
            RecipientId = recipientId,
            Channel = channel,
            Recipient = recipient,
            Subject = subject,
            Body = body,
            EventType = eventType,
            Status = NotificationStatus.Pending,
            RetryCount = 0
        };
    }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        LastAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed()
    {
        Status = NotificationStatus.Failed;
        LastAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementRetry()
    {
        RetryCount++;
        LastAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
