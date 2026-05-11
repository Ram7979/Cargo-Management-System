using CMS.NotificationService.Application.Commands.QueueNotification;
using CMS.NotificationService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.EventHandlers;

public class UserRegisteredEventHandler : INotificationHandler<UserRegisteredDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<UserRegisteredEventHandler> _logger;

    public UserRegisteredEventHandler(IMediator mediator, ILogger<UserRegisteredEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(UserRegisteredDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling UserRegisteredEvent for user {UserId}", notification.UserId);

        var request = new QueueNotificationRequest
        {
            RecipientId = notification.UserId,
            Channel = "Email",
            Recipient = notification.Email,
            Subject = "Welcome to Cargo Management System",
            Body = $"Hello {notification.FullName},\n\nWelcome to the Cargo Management System! Your account has been created successfully.",
            EventType = "UserRegistered"
        };

        await _mediator.Send(new QueueNotificationCommand(request), cancellationToken);
    }
}
