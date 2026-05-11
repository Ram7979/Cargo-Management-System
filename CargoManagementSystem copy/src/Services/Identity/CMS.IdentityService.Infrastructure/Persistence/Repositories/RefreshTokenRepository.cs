using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.IdentityService.Infrastructure.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly IdentityDbContext _context;

    public RefreshTokenRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(r => r.Token == token);
    }

    public async Task<IEnumerable<RefreshToken>> GetByFamilyAsync(string tokenFamily)
    {
        return await _context.RefreshTokens
            .Where(r => r.TokenFamily == tokenFamily)
            .ToListAsync();
    }

    public async Task AddAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(RefreshToken token)
    {
        _context.RefreshTokens.Update(token);
        await _context.SaveChangesAsync();
    }

    public async Task RevokeAllInFamilyAsync(string tokenFamily)
    {
        var tokens = await _context.RefreshTokens
            .Where(r => r.TokenFamily == tokenFamily)
            .ToListAsync();

        foreach (var token in tokens)
            token.Revoke();

        await _context.SaveChangesAsync();
    }
}
