using CMS.NotificationService.Domain.Entities;

namespace CMS.NotificationService.Domain.Interfaces;

public interface INotificationRepository
{
    Task<NotificationRecord?> GetByIdAsync(Guid id);
    Task<(IEnumerable<NotificationRecord> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        string? channel = null,
        string? recipientId = null,
        string? eventType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null);
    Task<IEnumerable<NotificationRecord>> GetFailedAsync();
    Task<IEnumerable<NotificationRecord>> GetUnreadByRecipientAsync(string recipientId);
    Task AddAsync(NotificationRecord record);
    Task UpdateAsync(NotificationRecord record);
    Task DeleteAsync(Guid id);
}
