using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.BillingService.Infrastructure.Persistence.Repositories;

public class RateCardRepository : IRateCardRepository
{
    private readonly BillingDbContext _context;

    public RateCardRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<RateCard?> GetByIdAsync(Guid id)
        => await _context.RateCards.FirstOrDefaultAsync(r => r.Id == id);

    public async Task<IEnumerable<RateCard>> GetAllAsync(string? serviceType = null, bool activeOnly = true)
    {
        var query = _context.RateCards.AsQueryable();
        if (activeOnly) query = query.Where(r => r.IsActive);
        if (!string.IsNullOrWhiteSpace(serviceType))
            query = query.Where(r => r.ServiceType.ToLower() == serviceType.ToLower());
        return await query.OrderBy(r => r.ServiceType).ToListAsync();
    }

    public async Task<RateCard?> GetBestMatchAsync(string serviceType, string zoneFrom, string zoneTo)
    {
        var now = DateTime.UtcNow;
        return await _context.RateCards
            .Where(r => r.IsActive &&
                r.ServiceType.ToLower() == serviceType.ToLower() &&
                r.EffectiveFrom <= now &&
                (r.EffectiveTo == null || r.EffectiveTo >= now))
            .OrderByDescending(r => r.EffectiveFrom)
            .FirstOrDefaultAsync();
    }

    public async Task AddAsync(RateCard rateCard)
    {
        await _context.RateCards.AddAsync(rateCard);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RateCard rateCard)
    {
        _context.RateCards.Update(rateCard);
        await _context.SaveChangesAsync();
    }
}
