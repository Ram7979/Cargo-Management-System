using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Enums;
using CMS.NotificationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationRepository : INotificationRepository
{
    private readonly NotificationDbContext _context;

    public NotificationRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationRecord?> GetByIdAsync(Guid id)
        => await _context.NotificationRecords.FirstOrDefaultAsync(n => n.Id == id);

    public async Task<(IEnumerable<NotificationRecord> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        string? channel = null,
        string? recipientId = null,
        string? eventType = null,
        DateTime? fromDate = null,
        DateTime? toDate = null)
    {
        var query = _context.NotificationRecords.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<NotificationStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(n => n.Status == parsedStatus);

        if (!string.IsNullOrWhiteSpace(channel) &&
            Enum.TryParse<NotificationChannel>(channel, ignoreCase: true, out var parsedChannel))
            query = query.Where(n => n.Channel == parsedChannel);

        if (!string.IsNullOrWhiteSpace(recipientId))
            query = query.Where(n => n.RecipientId == recipientId);

        if (!string.IsNullOrWhiteSpace(eventType))
            query = query.Where(n => n.EventType == eventType);

        if (fromDate.HasValue)
            query = query.Where(n => n.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(n => n.CreatedAt <= toDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<NotificationRecord>> GetFailedAsync()
        => await _context.NotificationRecords
            .Where(n => n.Status == NotificationStatus.Failed)
            .OrderByDescending(n => n.CreatedAt)
            .ToListAsync();

    public async Task<IEnumerable<NotificationRecord>> GetUnreadByRecipientAsync(string recipientId)
        => await _context.NotificationRecords
            .Where(n => n.RecipientId == recipientId && !n.IsRead)
            .ToListAsync();

    public async Task AddAsync(NotificationRecord record)
    {
        await _context.NotificationRecords.AddAsync(record);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NotificationRecord record)
    {
        _context.NotificationRecords.Update(record);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(Guid id)
    {
        var record = await _context.NotificationRecords.FirstOrDefaultAsync(n => n.Id == id);
        if (record != null)
        {
            record.SoftDelete();
            _context.NotificationRecords.Update(record);
            await _context.SaveChangesAsync();
        }
    }
}
