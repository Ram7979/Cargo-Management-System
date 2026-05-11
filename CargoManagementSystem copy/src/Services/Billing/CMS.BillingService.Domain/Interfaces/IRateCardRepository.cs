using CMS.BillingService.Domain.Entities;

namespace CMS.BillingService.Domain.Interfaces;

public interface IRateCardRepository
{
    Task<RateCard?> GetByIdAsync(Guid id);
    Task<IEnumerable<RateCard>> GetAllAsync(string? serviceType = null, bool activeOnly = true);
    Task<RateCard?> GetBestMatchAsync(string serviceType, string zoneFrom, string zoneTo);
    Task AddAsync(RateCard rateCard);
    Task UpdateAsync(RateCard rateCard);
}
