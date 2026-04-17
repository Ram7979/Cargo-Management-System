namespace CMS.NotificationService.Application.DTOs;

public class NotificationDto
{
    public Guid Id { get; set; }
    public string RecipientId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Status { get; set; } = string.Empty;
    public int RetryCount { get; set; }
    public DateTime? LastAttemptAt { get; set; }
    public string EventType { get; set; } = string.Empty;
    public bool IsRead { get; set; }
    public DateTime? ReadAt { get; set; }
    public string? ErrorMessage { get; set; }
    public DateTime CreatedAt { get; set; }
}

public class NotificationPreferenceDto
{
    public string RecipientId { get; set; } = string.Empty;
    public bool EmailEnabled { get; set; }
    public bool SmsEnabled { get; set; }
    public bool PushEnabled { get; set; }
    public IEnumerable<string> OptedOutEventTypes { get; set; } = new List<string>();
}

public class UpdatePreferenceRequest
{
    public bool EmailEnabled { get; set; } = true;
    public bool SmsEnabled { get; set; } = true;
    public bool PushEnabled { get; set; } = false;
    public IEnumerable<string>? OptedOutEventTypes { get; set; }
}
