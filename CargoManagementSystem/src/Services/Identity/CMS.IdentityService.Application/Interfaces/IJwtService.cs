using CMS.IdentityService.Domain.Entities;

namespace CMS.IdentityService.Application.Interfaces;

public interface IJwtService
{
    string GenerateAccessToken(User user);
    string GenerateRefreshToken();
    string GetTokenFamily();
}
