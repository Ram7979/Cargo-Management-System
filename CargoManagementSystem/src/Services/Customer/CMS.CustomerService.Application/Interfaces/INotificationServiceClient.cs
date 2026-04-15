namespace CMS.CustomerService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task SendWelcomeEmailAsync(string recipientId, string email, string fullName);
}
