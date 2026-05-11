using System;
using System.Threading;
using System.Threading.Tasks;
using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.ShipmentService.Application.Events.Handlers;

public class ShipmentCreatedEventHandler : INotificationHandler<ShipmentCreatedEvent>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly IBillingServiceClient _billingServiceClient;
    private readonly INotificationServiceClient _notificationServiceClient;
    private readonly ILogger<ShipmentCreatedEventHandler> _logger;

    public ShipmentCreatedEventHandler(
        IShipmentRepository shipmentRepository,
        IBillingServiceClient billingServiceClient,
        INotificationServiceClient notificationServiceClient,
        ILogger<ShipmentCreatedEventHandler> logger)
    {
        _shipmentRepository = shipmentRepository;
        _billingServiceClient = billingServiceClient;
        _notificationServiceClient = notificationServiceClient;
        _logger = logger;
    }

    public async Task Handle(ShipmentCreatedEvent notification, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Processing ShipmentCreatedEvent for shipment {ShipmentId}", notification.ShipmentId);

        var shipment = await _shipmentRepository.GetByIdAsync(notification.ShipmentId);
        if (shipment == null)
        {
            _logger.LogWarning("Shipment {ShipmentId} not found, unable to generate invoice.", notification.ShipmentId);
            return;
        }

        // Basic calculation logic for invoice (ideally uses CalculateRate endpoint, but this provides a fallback)
        var baseRate = shipment.ServiceType.ToLower() == "express" ? 50m : 20m;
        var weightCharge = shipment.WeightKg * 2.5m;
        var volumeCharge = shipment.VolumeCbm * 10m;
        
        var baseFreightCharge = baseRate + Math.Max(weightCharge, volumeCharge);
        var fuelSurcharge = baseFreightCharge * 0.15m;
        var handlingFee = 15m;
        var insuranceAmount = shipment.DeclaredValue * 0.01m;

        var request = new GenerateInvoiceInternalRequest
        {
            ShipmentId = shipment.Id,
            CustomerId = shipment.CustomerId,
            BaseFreightCharge = baseFreightCharge,
            FuelSurcharge = fuelSurcharge,
            HandlingFee = handlingFee,
            InsuranceAmount = insuranceAmount,
            TaxRate = 0.18m,
            DueDate = DateTime.UtcNow.AddDays(30),
            Notes = $"Auto-generated for {shipment.ServiceType} shipment."
        };

        var success = await _billingServiceClient.GenerateInvoiceAsync(request);
        if (success)
        {
            _logger.LogInformation("Successfully auto-generated invoice for shipment {ShipmentId}", shipment.Id);
        }
        else
        {
            _logger.LogError("Failed to auto-generate invoice for shipment {ShipmentId}", shipment.Id);
        }

        // Send a notification to the customer about their newly booked shipment
        try
        {
            await _notificationServiceClient.QueueNotificationAsync(
                recipientId: shipment.CustomerId,
                channel: "Push",
                recipient: shipment.CustomerId, // They use recipientId as routing fallback
                subject: "Shipment Booked Successfully",
                body: $"Your shipment {shipment.TrackingNumber} has been successfully booked on {DateTime.UtcNow:yyyy-MM-dd}. Status: {shipment.Status}.",
                eventType: "ShipmentUpdate"
            );
            _logger.LogInformation("Successfully queued InApp notification for shipment {TrackingNumber}", shipment.TrackingNumber);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to queue notification for shipment {TrackingNumber}", shipment.TrackingNumber);
        }
    }
}
