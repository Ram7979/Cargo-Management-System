namespace CMS.NotificationService.Application.Interfaces;

public interface ISmsService
{
    Task<bool> SendAsync(string to, string body);
}
