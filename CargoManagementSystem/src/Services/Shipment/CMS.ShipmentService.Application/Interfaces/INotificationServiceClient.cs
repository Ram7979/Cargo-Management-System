namespace CMS.ShipmentService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task QueueNotificationAsync(
        string recipientId,
        string channel,
        string recipient,
        string subject,
        string body,
        string eventType);
}
