using CMS.FleetService.Application.Events;
using CMS.FleetService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.FleetService.Application.EventHandlers;

public class ShipmentAssignedEventHandler : INotificationHandler<ShipmentAssignedEvent>
{
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly ILogger<ShipmentAssignedEventHandler> _logger;

    public ShipmentAssignedEventHandler(
        IShipmentServiceClient shipmentServiceClient,
        ILogger<ShipmentAssignedEventHandler> logger)
    {
        _shipmentServiceClient = shipmentServiceClient;
        _logger = logger;
    }

    public async Task Handle(ShipmentAssignedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _shipmentServiceClient.UpdateShipmentStatusAsync(
                notification.ShipmentId, "Assigned", "FleetService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update shipment {ShipmentId} status to Assigned", notification.ShipmentId);
            // Don't rethrow — assignment is already created, status update is best-effort
        }
    }
}
