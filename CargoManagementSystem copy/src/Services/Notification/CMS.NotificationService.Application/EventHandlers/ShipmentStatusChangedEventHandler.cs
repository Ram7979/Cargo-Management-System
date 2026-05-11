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

        var variables = new Dictionary<string, string>
        {
            { "CustomerName", notification.CustomerName },
            { "TrackingNumber", notification.TrackingNumber },
            { "OldStatus", notification.OldStatus },
            { "NewStatus", notification.NewStatus },
            { "ETA", notification.EstimatedDelivery?.ToString("yyyy-MM-dd") ?? "TBD" }
        };

        var defaultSubject = $"Shipment {notification.TrackingNumber} Status Update";
        var defaultBody = $"Dear {notification.CustomerName},\n\nYour shipment {notification.TrackingNumber} has been updated from {notification.OldStatus} to {notification.NewStatus}.\n\nEstimated Delivery: {variables["ETA"]}";

        // Queue Email notification
        if (!string.IsNullOrWhiteSpace(notification.RecipientEmail))
        {
            await _mediator.Send(new QueueNotificationCommand(new QueueNotificationRequest
            {
                RecipientId = notification.CustomerId,
                Channel = "Email",
                Recipient = notification.RecipientEmail,
                Subject = defaultSubject,
                Body = defaultBody,
                EventType = "ShipmentStatusChanged",
                TemplateVariables = variables
            }), cancellationToken);
        }

        // Queue SMS notification
        if (!string.IsNullOrWhiteSpace(notification.RecipientPhone))
        {
            await _mediator.Send(new QueueNotificationCommand(new QueueNotificationRequest
            {
                RecipientId = notification.CustomerId,
                Channel = "SMS",
                Recipient = notification.RecipientPhone,
                Subject = defaultSubject,
                Body = $"CMS: Shipment {notification.TrackingNumber} is now {notification.NewStatus}. ETA: {variables["ETA"]}",
                EventType = "ShipmentStatusChanged",
                TemplateVariables = variables
            }), cancellationToken);
        }
        // Queue Push notification (for UI Dashboard)
        await _mediator.Send(new QueueNotificationCommand(new QueueNotificationRequest
        {
            RecipientId = notification.CustomerId,
            Channel = "Push",
            Recipient = notification.CustomerId, // System ID for push
            Subject = defaultSubject,
            Body = $"Your shipment {notification.TrackingNumber} has been updated to {notification.NewStatus}.",
            EventType = "ShipmentStatusChanged",
            TemplateVariables = variables
        }), cancellationToken);
    }
}
