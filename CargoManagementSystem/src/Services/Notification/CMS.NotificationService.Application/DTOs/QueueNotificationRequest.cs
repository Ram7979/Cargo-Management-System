namespace CMS.NotificationService.Application.DTOs;

public class QueueNotificationRequest
{
    public string RecipientId { get; set; } = string.Empty;
    public string Channel { get; set; } = string.Empty;
    public string Recipient { get; set; } = string.Empty;
    public string Subject { get; set; } = string.Empty;
    public string Body { get; set; } = string.Empty;
    public string EventType { get; set; } = string.Empty;
}
