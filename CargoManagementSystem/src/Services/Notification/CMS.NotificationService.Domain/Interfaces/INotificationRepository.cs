using CMS.NotificationService.Domain.Entities;

namespace CMS.NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task<NotificationRecord?> GetByIdAsync(Guid id);
    Task<(IEnumerable<NotificationRecord> Items, int TotalCount)> GetPagedAsync(int page, int pageSize);
    Task AddAsync(NotificationRecord record);
    Task UpdateAsync(NotificationRecord record);
}
