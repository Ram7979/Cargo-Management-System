using CMS.NotificationService.Application.Commands.QueueNotification;
using CMS.NotificationService.Application.DTOs;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.NotificationService.Application.EventHandlers;

public class DamageReportedEventHandler : INotificationHandler<DamageReportedDomainEvent>
{
    private readonly IMediator _mediator;
    private readonly ILogger<DamageReportedEventHandler> _logger;

    public DamageReportedEventHandler(IMediator mediator, ILogger<DamageReportedEventHandler> logger)
    {
        _mediator = mediator;
        _logger = logger;
    }

    public async Task Handle(DamageReportedDomainEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling DamageReportedEvent for shipment {ShipmentId}", notification.ShipmentId);

        var request = new QueueNotificationRequest
        {
            RecipientId = "ops-manager",
            Channel = "Email",
            Recipient = notification.OpsManagerEmail,
            Subject = $"Damage Alert: Shipment {notification.ShipmentId}",
            Body = $"Damage has been reported for shipment {notification.ShipmentId} (Receipt: {notification.ReceiptId}).\n\nDamage Notes: {notification.DamageNotes}",
            EventType = "DamageReported"
        };

        await _mediator.Send(new QueueNotificationCommand(request), cancellationToken);
    }
}
