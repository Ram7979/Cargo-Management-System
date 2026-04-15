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

namespace CMS.WarehouseService.Application.Commands.ReceiveCargo;

public class ReceiveCargoCommandHandler : IRequestHandler<ReceiveCargoCommand, ApiResponse<CargoReceiptDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBinRepository _binRepository;
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public ReceiveCargoCommandHandler(
        IWarehouseRepository warehouseRepository,
        IBinRepository binRepository,
        ICargoReceiptRepository receiptRepository,
        IShipmentServiceClient shipmentServiceClient,
        IMediator mediator,
        IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _binRepository = binRepository;
        _receiptRepository = receiptRepository;
        _shipmentServiceClient = shipmentServiceClient;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CargoReceiptDto>> Handle(ReceiveCargoCommand command, CancellationToken cancellationToken)
    {
        // 1. Get warehouse by id
        var warehouse = await _warehouseRepository.GetByIdAsync(command.Request.WarehouseId)
            ?? throw new NotFoundException("Warehouse", command.Request.WarehouseId);

        // 2. Validate shipment status
        var shipmentStatus = await _shipmentServiceClient.GetShipmentStatusAsync(command.Request.ShipmentId);
        if (shipmentStatus != "PickedUp" && shipmentStatus != "InTransit")
            throw new UnprocessableException(
                $"Shipment is in status '{shipmentStatus}'. Only 'PickedUp' or 'InTransit' shipments can be received at warehouse.");

        // 3. Get available bin
        var bin = await _binRepository.GetAvailableAsync(command.Request.WarehouseId)
            ?? throw new NoBinAvailableException($"No available bin in warehouse '{command.Request.WarehouseId}'.");

        // 4. Mark bin as occupied
        bin.Occupy();

        // 5. Create CargoReceipt
        var receipt = CargoReceipt.Create(
            command.Request.ShipmentId,
            command.Request.WarehouseId,
            bin.Id,
            command.ActorUserId,
            command.Request.HasDamageReport,
            command.Request.DamageNotes ?? string.Empty);

        // 6. Save receipt and update bin
        await _receiptRepository.AddAsync(receipt);
        await _binRepository.UpdateAsync(bin);

        // 7. Publish CargoReceivedEvent (triggers shipment status → AtWarehouse)
        await _mediator.Publish(
            new CargoReceivedEvent(receipt.ShipmentId, receipt.WarehouseId, receipt.BinId, receipt.Id),
            cancellationToken);

        // 8. If HasDamageReport, publish DamageReportedEvent
        if (command.Request.HasDamageReport)
        {
            await _mediator.Publish(
                new DamageReportedEvent(receipt.ShipmentId, receipt.Id, receipt.DamageNotes),
                cancellationToken);
        }

        // 9. Return CargoReceiptDto
        var dto = _mapper.Map<CargoReceiptDto>(receipt);
        return ApiResponse<CargoReceiptDto>.Ok(dto, "Cargo received successfully.");
    }
}
