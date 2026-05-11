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

    public async Task<User?> GetByIdAsync(Guid id)
    {
        // Users table contains ApplicationUser (IdentityUser) entities
        // This repository bridges to the domain User entity
        // For now, return null — the AuthController uses UserManager<ApplicationUser> directly
        return null;
    }

    public async Task<User?> GetByEmailAsync(string email)
    {
        return null;
    }

    public async Task<IEnumerable<User>> GetAllAsync()
    {
        return Enumerable.Empty<User>();
    }

    public async Task AddAsync(User user)
    {
        // No-op: AuthController uses UserManager<ApplicationUser> for user creation
        await Task.CompletedTask;
    }

    public async Task UpdateAsync(User user)
    {
        await Task.CompletedTask;
    }

    public async Task<bool> ExistsAsync(string email)
    {
        return await _context.Users.AnyAsync(u => u.Email == email);
    }
}
