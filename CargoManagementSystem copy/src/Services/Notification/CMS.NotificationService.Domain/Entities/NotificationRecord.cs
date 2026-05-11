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
    public bool IsRead { get; private set; }
    public DateTime? ReadAt { get; private set; }
    public string? ErrorMessage { get; private set; }
    public bool IsDeleted { get; private set; }
    public DateTime? DeletedAt { get; private set; }

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
            RetryCount = 0,
            IsRead = false,
            IsDeleted = false
        };
    }

    public void MarkSent()
    {
        Status = NotificationStatus.Sent;
        LastAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkFailed(string? errorMessage = null)
    {
        Status = NotificationStatus.Failed;
        LastAttemptAt = DateTime.UtcNow;
        ErrorMessage = errorMessage;
        UpdatedAt = DateTime.UtcNow;
    }

    public void IncrementRetry()
    {
        RetryCount++;
        LastAttemptAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void MarkAsRead()
    {
        IsRead = true;
        ReadAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    public void SoftDelete()
    {
        IsDeleted = true;
        DeletedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }
}
