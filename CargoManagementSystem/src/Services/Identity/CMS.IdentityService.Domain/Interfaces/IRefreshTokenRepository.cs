using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Domain.Interfaces;

public interface IRefreshTokenRepository
{
    Task<RefreshToken?> GetByTokenAsync(string token);
    Task<IEnumerable<RefreshToken>> GetByFamilyAsync(string tokenFamily);
    Task AddAsync(RefreshToken token);
    Task UpdateAsync(RefreshToken token);
    Task RevokeAllInFamilyAsync(string tokenFamily);
}
