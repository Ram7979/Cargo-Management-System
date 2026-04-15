using CMS.WarehouseService.Application.Events;
using CMS.WarehouseService.Application.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.EventHandlers;

public class CargoReceivedEventHandler : INotificationHandler<CargoReceivedEvent>
{
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly ILogger<CargoReceivedEventHandler> _logger;

    public CargoReceivedEventHandler(
        IShipmentServiceClient shipmentServiceClient,
        ILogger<CargoReceivedEventHandler> logger)
    {
        _shipmentServiceClient = shipmentServiceClient;
        _logger = logger;
    }

    public async Task Handle(CargoReceivedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            await _shipmentServiceClient.UpdateShipmentStatusAsync(
                notification.ShipmentId, "AtWarehouse", "WarehouseService");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update shipment {ShipmentId} status to AtWarehouse", notification.ShipmentId);
        }
    }
}
