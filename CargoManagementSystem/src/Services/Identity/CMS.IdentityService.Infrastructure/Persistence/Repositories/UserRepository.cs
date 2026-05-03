using CMS.IdentityService.Domain.Entities;
using CMS.IdentityService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.IdentityService.Infrastructure.Persistence.Repositories;

public class UserRepository : IUserRepository
{
    private readonly IdentityDbContext _context;

    public UserRepository(IdentityDbContext context)
    {
        _context = context;
    }

    public async Task<ApplicationUser?> GetByIdAsync(string id)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Id == id);
    }

    public async Task<ApplicationUser?> GetByEmailAsync(string email)
    {
        return await _context.Users
            .FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync()
    {
        return await _context.Users
            .ToListAsync();
    }

    public async Task AddAsync(ApplicationUser user)
    {
        await _context.Users.AddAsync(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(ApplicationUser user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
