using CMS.Shared.Entities;

namespace CMS.IdentityService.Domain.Entities;

public class RefreshToken : BaseEntity
{
    public string UserId { get; private set; } = string.Empty;
    public string Token { get; private set; } = string.Empty;
    public string TokenFamily { get; private set; } = string.Empty;
    public DateTime ExpiresAt { get; private set; }
    public bool IsUsed { get; private set; }
    public bool IsRevoked { get; private set; }

    private RefreshToken() { }

    public static RefreshToken Create(string userId, string token, string tokenFamily, DateTime expiresAt)
    {
        return new RefreshToken
        {
            UserId = userId,
            Token = token,
            TokenFamily = tokenFamily,
            ExpiresAt = expiresAt,
            IsUsed = false,
            IsRevoked = false
        };
    }

    public void MarkUsed() => IsUsed = true;

    public void Revoke() => IsRevoked = true;

    public bool IsActive() => !IsUsed && !IsRevoked && ExpiresAt > DateTime.UtcNow;
}
