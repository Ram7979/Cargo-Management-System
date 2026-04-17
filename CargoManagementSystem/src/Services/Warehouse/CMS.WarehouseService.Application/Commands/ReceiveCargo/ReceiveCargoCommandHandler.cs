using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Events;
using CMS.WarehouseService.Application.Interfaces;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Exceptions;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.Commands.ReceiveCargo;

public class ReceiveCargoCommandHandler : IRequestHandler<ReceiveCargoCommand, ApiResponse<CargoReceiptDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBinRepository _binRepository;
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly INotificationServiceClient _notificationClient;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;
    private readonly ILogger<ReceiveCargoCommandHandler> _logger;

    public ReceiveCargoCommandHandler(
        IWarehouseRepository warehouseRepository,
        IBinRepository binRepository,
        ICargoReceiptRepository receiptRepository,
        IShipmentServiceClient shipmentServiceClient,
        INotificationServiceClient notificationClient,
        IMediator mediator,
        IMapper mapper,
        ILogger<ReceiveCargoCommandHandler> logger)
    {
        _warehouseRepository = warehouseRepository;
        _binRepository = binRepository;
        _receiptRepository = receiptRepository;
        _shipmentServiceClient = shipmentServiceClient;
        _notificationClient = notificationClient;
        _mediator = mediator;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<CargoReceiptDto>> Handle(ReceiveCargoCommand command, CancellationToken cancellationToken)
    {
        // 1. Resolve shipment by tracking number
        var shipmentDetail = await _shipmentServiceClient.GetShipmentByTrackingNumberAsync(command.Request.TrackingNumber)
            ?? throw new NotFoundException("Shipment", command.Request.TrackingNumber);

        // 2. Get warehouse
        var warehouse = await _warehouseRepository.GetByIdAsync(command.Request.WarehouseId)
            ?? throw new NotFoundException("Warehouse", command.Request.WarehouseId);

        // 3. Validate shipment status
        if (shipmentDetail.Status != "PickedUp" && shipmentDetail.Status != "InTransit")
            throw new UnprocessableException(
                $"Shipment is in status '{shipmentDetail.Status}'. Only 'PickedUp' or 'InTransit' shipments can be received.");

        // 4. Resolve bin — operator-selected or auto-assign
        Bin bin;
        if (command.Request.BinId.HasValue)
        {
            bin = await _binRepository.GetByIdAsync(command.Request.BinId.Value)
                ?? throw new NotFoundException("Bin", command.Request.BinId.Value);
            if (bin.IsOccupied)
                throw new UnprocessableException($"Bin '{bin.BinCode}' is already occupied.");
            if (!bin.IsActive)
                throw new UnprocessableException($"Bin '{bin.BinCode}' is not active.");
            if (bin.WarehouseId != command.Request.WarehouseId)
                throw new UnprocessableException("Selected bin does not belong to this warehouse.");
        }
        else
        {
            bin = await _binRepository.GetAvailableAsync(command.Request.WarehouseId, shipmentDetail.WeightKg)
                ?? throw new NoBinAvailableException($"No available bin with sufficient capacity in warehouse '{command.Request.WarehouseId}'.");
        }

        bin.Occupy();

        // 5. Create CargoReceipt
        var receipt = CargoReceipt.Create(
            shipmentDetail.ShipmentId,
            command.Request.TrackingNumber,
            command.Request.WarehouseId,
            bin.Id,
            command.ActorUserId,
            command.Request.HasDamageReport,
            command.Request.DamageNotes ?? string.Empty,
            command.Request.Remarks ?? string.Empty);

        await _receiptRepository.AddAsync(receipt);
        await _binRepository.UpdateAsync(bin);

        // 6. Publish CargoReceivedEvent → updates shipment status to AtWarehouse
        await _mediator.Publish(
            new CargoReceivedEvent(receipt.ShipmentId, receipt.WarehouseId, receipt.BinId, receipt.Id),
            cancellationToken);

        // 7. Publish DamageReportedEvent if damaged
        if (command.Request.HasDamageReport)
        {
            await _mediator.Publish(
                new DamageReportedEvent(receipt.ShipmentId, receipt.Id, receipt.DamageNotes),
                cancellationToken);
        }

        // 8. Notify dispatcher and customer
        try
        {
            await _notificationClient.SendWarehouseArrivalNotificationAsync(
                shipmentDetail.ShipmentId.ToString(),
                shipmentDetail.ShipmentId,
                command.Request.TrackingNumber,
                warehouse.Name);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send warehouse arrival notification for {TrackingNumber}", command.Request.TrackingNumber);
        }

        var dto = _mapper.Map<CargoReceiptDto>(receipt);
        return ApiResponse<CargoReceiptDto>.Ok(dto, "Cargo received successfully.");
    }
}
