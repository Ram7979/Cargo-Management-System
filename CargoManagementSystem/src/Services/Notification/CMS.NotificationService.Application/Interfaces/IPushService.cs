namespace CMS.NotificationService.Application.Interfaces;

public interface IPushService
{
    Task<bool> SendAsync(string deviceToken, string title, string body);
}
