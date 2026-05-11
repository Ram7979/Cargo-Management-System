using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Enums;
using CMS.NotificationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationTemplateRepository : INotificationTemplateRepository
{
    private readonly NotificationDbContext _context;

    public NotificationTemplateRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationTemplate?> GetByEventTypeAndChannelAsync(string eventType, NotificationChannel channel)
        => await _context.NotificationTemplates
            .FirstOrDefaultAsync(t => t.EventType == eventType && t.Channel == channel && t.IsActive);

    public async Task<IEnumerable<NotificationTemplate>> GetAllAsync()
        => await _context.NotificationTemplates.OrderBy(t => t.EventType).ToListAsync();

    public async Task AddAsync(NotificationTemplate template)
    {
        await _context.NotificationTemplates.AddAsync(template);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NotificationTemplate template)
    {
        _context.NotificationTemplates.Update(template);
        await _context.SaveChangesAsync();
    }
}
