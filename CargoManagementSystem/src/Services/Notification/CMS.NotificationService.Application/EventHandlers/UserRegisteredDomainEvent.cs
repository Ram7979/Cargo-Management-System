using CMS.Shared.Events;

namespace CMS.NotificationService.Application.EventHandlers;

public class UserRegisteredDomainEvent : IDomainEvent
{
    public string UserId { get; }
    public string Email { get; }
    public string FullName { get; }

    public UserRegisteredDomainEvent(string userId, string email, string fullName)
    {
        UserId = userId;
        Email = email;
        FullName = fullName;
    }
}
