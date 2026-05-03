using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Domain.Interfaces;

public interface IUserRepository
{
    Task<ApplicationUser?> GetByIdAsync(string id);
    Task<ApplicationUser?> GetByEmailAsync(string email);
    Task<IEnumerable<ApplicationUser>> GetAllAsync();
    Task AddAsync(ApplicationUser user);
    Task UpdateAsync(ApplicationUser user);
    Task<bool> ExistsAsync(string email);
}
