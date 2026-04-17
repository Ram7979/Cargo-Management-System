using CMS.NotificationService.Domain.Entities;
using CMS.NotificationService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.NotificationService.Infrastructure.Persistence.Repositories;

public class NotificationPreferenceRepository : INotificationPreferenceRepository
{
    private readonly NotificationDbContext _context;

    public NotificationPreferenceRepository(NotificationDbContext context)
    {
        _context = context;
    }

    public async Task<NotificationPreference?> GetByRecipientIdAsync(string recipientId)
        => await _context.NotificationPreferences
            .FirstOrDefaultAsync(p => p.RecipientId == recipientId);

    public async Task AddAsync(NotificationPreference preference)
    {
        await _context.NotificationPreferences.AddAsync(preference);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(NotificationPreference preference)
    {
        _context.NotificationPreferences.Update(preference);
        await _context.SaveChangesAsync();
    }
}
