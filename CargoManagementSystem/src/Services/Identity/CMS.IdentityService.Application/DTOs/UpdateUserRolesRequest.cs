namespace CMS.IdentityService.Application.DTOs;

public class UpdateUserRolesRequest
{
    public IEnumerable<string> Roles { get; set; } = Enumerable.Empty<string>();
}
