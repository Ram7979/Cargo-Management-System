using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Enums;

namespace CMS.NotificationService.Domain.Interfaces;

public interface INotificationTemplateRepository
{
    Task<NotificationTemplate?> GetByEventTypeAndChannelAsync(string eventType, NotificationChannel channel);
    Task<IEnumerable<NotificationTemplate>> GetAllAsync();
    Task AddAsync(NotificationTemplate template);
    Task UpdateAsync(NotificationTemplate template);
}
