using CMS.WarehouseService.Application.Events;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.EventHandlers;

public class DamageReportedEventHandler : INotificationHandler<DamageReportedEvent>
{
    private readonly ILogger<DamageReportedEventHandler> _logger;

    public DamageReportedEventHandler(ILogger<DamageReportedEventHandler> logger)
    {
        _logger = logger;
    }

    public async Task Handle(DamageReportedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogWarning(
            "Damage reported for shipment {ShipmentId}, receipt {ReceiptId}: {Notes}",
            notification.ShipmentId, notification.ReceiptId, notification.DamageNotes);
        await Task.CompletedTask;
    }
}
