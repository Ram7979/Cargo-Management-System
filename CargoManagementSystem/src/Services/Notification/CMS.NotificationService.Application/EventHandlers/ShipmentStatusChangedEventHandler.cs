using CMS.NotificationService.Application.Commands.QueueNotification;
using CMS.NotificationService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.EventHandlers;

public class ShipmentStatusChangedEventHandler : INotificationHandler<ShipmentStatusChangedDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<ShipmentStatusChangedEventHandler> _logger;

    public ShipmentStatusChangedEventHandler(IMediator mediator, ILogger<ShipmentStatusChangedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(ShipmentStatusChangedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ShipmentStatusChangedEvent for shipment {ShipmentId}", notification.ShipmentId);

        var request = new QueueNotificationRequest
        {
            RecipientId = notification.CustomerId,
            Channel = "Email",
            Recipient = notification.RecipientEmail,
            Subject = $"Shipment {notification.TrackingNumber} Status Update",
            Body = $"Your shipment {notification.TrackingNumber} has been updated from {notification.OldStatus} to {notification.NewStatus}.",
            EventType = "ShipmentStatusChanged"
        };

        await _mediator.Send(new QueueNotificationCommand(request), cancellationToken);
    }
}
