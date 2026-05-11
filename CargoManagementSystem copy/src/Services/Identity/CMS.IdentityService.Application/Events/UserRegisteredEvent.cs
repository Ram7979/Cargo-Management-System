using CMS.Shared.Events;

namespace CMS.IdentityService.Application.Events;

public class UserRegisteredEvent : IDomainEvent
{
    public Guid UserId { get; }
    public string Email { get; }
    public string FullName { get; }

    public UserRegisteredEvent(Guid userId, string email, string fullName)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
    }
}
