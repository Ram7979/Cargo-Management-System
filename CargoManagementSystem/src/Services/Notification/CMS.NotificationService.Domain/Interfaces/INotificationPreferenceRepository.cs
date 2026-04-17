using CMS.NotificationService.Domain.Entities;

namespace CMS.NotificationService.Domain.Interfaces;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> GetByRecipientIdAsync(string recipientId);
    Task AddAsync(NotificationPreference preference);
    Task UpdateAsync(NotificationPreference preference);
}
