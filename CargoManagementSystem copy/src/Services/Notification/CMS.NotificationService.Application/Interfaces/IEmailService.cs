namespace CMS.NotificationService.Application.Interfaces;

public interface IEmailService
{
    Task<bool> SendAsync(string to, string subject, string body);
}
