using System;
using System.Threading;
using System.Threading.Tasks;
using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.ShipmentService.Application.Events.Handlers;

public class ShipmentStatusChangedEventHandler : INotificationHandler<ShipmentStatusChangedEvent>
{
    private readonly INotificationServiceClient _notificationServiceClient;
    private readonly ILogger<ShipmentStatusChangedEventHandler> _logger;

    public ShipmentStatusChangedEventHandler(
        INotificationServiceClient notificationServiceClient,
        ILogger<ShipmentStatusChangedEventHandler> logger)
    {
        _notificationServiceClient = notificationServiceClient;
        _logger = logger;
    }

    public async Task Handle(ShipmentStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ShipmentStatusChangedEvent for shipment {ShipmentId} ({TrackingNumber})", notification.ShipmentId, notification.TrackingNumber);

        try
        {
            await _notificationServiceClient.QueueNotificationAsync(
                recipientId: notification.CustomerId,
                channel: "Push",
                recipient: notification.CustomerId,
                subject: "Shipment Status Update",
                body: $"Your shipment {notification.TrackingNumber} status has been updated from {notification.OldStatus} to {notification.NewStatus}.",
                eventType: "ShipmentUpdate"
            );
            
            _logger.LogInformation("Successfully queued status update notification for shipment {TrackingNumber}", notification.TrackingNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue status update notification for shipment {TrackingNumber}", notification.TrackingNumber);
        }
    }
}
