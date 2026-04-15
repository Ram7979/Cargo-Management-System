using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.ShipmentService.Application.EventHandlers;

public class ShipmentStatusChangedEventHandler : INotificationHandler<ShipmentStatusChangedEvent>
{
    private readonly INotificationServiceClient _notificationClient;
    private readonly ILogger<ShipmentStatusChangedEventHandler> _logger;

    public ShipmentStatusChangedEventHandler(
        INotificationServiceClient notificationClient,
        ILogger<ShipmentStatusChangedEventHandler> logger)
    {
        _notificationClient = notificationClient;
        _logger = logger;
    }

    public async Task Handle(ShipmentStatusChangedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _notificationClient.QueueNotificationAsync(
                notification.CustomerId.ToString(),
                "Email",
                "customer@example.com",
                $"Shipment {notification.TrackingNumber} Status Update",
                $"Your shipment {notification.TrackingNumber} status has changed from {notification.OldStatus} to {notification.NewStatus}.",
                "ShipmentStatusChanged");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue notification for shipment {ShipmentId}", notification.ShipmentId);
        }
    }
}
