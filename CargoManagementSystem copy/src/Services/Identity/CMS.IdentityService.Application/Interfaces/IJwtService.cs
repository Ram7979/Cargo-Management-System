using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(ApplicationUser user, IList<string> roles);
    string GenerateRefreshToken();
    string GetTokenFamily();
}
