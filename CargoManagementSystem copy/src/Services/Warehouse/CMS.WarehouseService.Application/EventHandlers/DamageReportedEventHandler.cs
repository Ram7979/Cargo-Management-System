using CMS.WarehouseService.Application.Events;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.EventHandlers;

public class DamageReportedEventHandler : INotificationHandler<DamageReportedEvent>
{
    private readonly IDamageReportRepository _damageReportRepository;
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly ILogger<DamageReportedEventHandler> _logger;

    public DamageReportedEventHandler(
        IDamageReportRepository damageReportRepository,
        ICargoReceiptRepository receiptRepository,
        ILogger<DamageReportedEventHandler> logger)
    {
        _damageReportRepository = damageReportRepository;
        _receiptRepository = receiptRepository;
        _logger = logger;
    }

    public async Task Handle(DamageReportedEvent notification, CancellationToken cancellationToken)
    {
        try
        {
            // Resolve the receipt to get warehouseId and reportedByUserId
            var receipt = await _receiptRepository.GetByIdAsync(notification.ReceiptId);
            if (receipt == null)
            {
                _logger.LogWarning(
                    "Cannot create DamageReport: receipt {ReceiptId} not found for shipment {ShipmentId}",
                    notification.ReceiptId, notification.ShipmentId);
                return;
            }

            var report = DamageReport.Create(
                notification.ShipmentId,
                notification.ReceiptId,
                receipt.WarehouseId,
                receipt.ReceivedByUserId,
                notification.DamageNotes);

            await _damageReportRepository.AddAsync(report);

            _logger.LogInformation(
                "DamageReport {ReportId} created for shipment {ShipmentId}, receipt {ReceiptId}",
                report.Id, notification.ShipmentId, notification.ReceiptId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to persist DamageReport for shipment {ShipmentId}, receipt {ReceiptId}",
                notification.ShipmentId, notification.ReceiptId);
        }
    }
}
