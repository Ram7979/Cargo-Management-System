using CMS.IdentityService.Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.IdentityService.Application.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredEvent>
{
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(ILogger<UserRegisteredEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(UserRegisteredEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation(
            "Welcome email queued for user {UserId} ({Email})",
            notification.UserId, notification.Email);
        await Task.CompletedTask;
    }
}
