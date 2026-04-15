namespace CMS.IdentityService.Domain.Entities;

public class UserRole
{
    public Guid UserId { get; private set; }
    public string RoleName { get; private set; } = string.Empty;

    private UserRole() { }

    public UserRole(Guid userId, string roleName)
    {
        UserId = userId;
        RoleName = roleName;
    }
}
