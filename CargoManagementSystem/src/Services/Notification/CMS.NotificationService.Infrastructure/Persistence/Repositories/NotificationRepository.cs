using CMS.NotificationService.Domain.Entities;
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
    {
        return await _context.NotificationRecords.FirstOrDefaultAsync(n => n.Id == id);
    }

    public async Task<(IEnumerable<NotificationRecord> Items, int TotalCount)> GetPagedAsync(int page, int pageSize)
    {
        var query = _context.NotificationRecords.AsQueryable();

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(n => n.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

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
}
