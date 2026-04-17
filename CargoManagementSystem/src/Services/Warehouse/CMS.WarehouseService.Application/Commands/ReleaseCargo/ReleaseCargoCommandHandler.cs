using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Interfaces;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.WarehouseService.Application.Commands.ReleaseCargo;

public class ReleaseCargoCommandHandler : IRequestHandler<ReleaseCargoCommand, ApiResponse<CargoReceiptDto>>
{
    private readonly ICargoReceiptRepository _receiptRepository;
    private readonly IBinRepository _binRepository;
    private readonly IShipmentServiceClient _shipmentServiceClient;
    private readonly IMapper _mapper;
    private readonly ILogger<ReleaseCargoCommandHandler> _logger;

    public ReleaseCargoCommandHandler(
        ICargoReceiptRepository receiptRepository,
        IBinRepository binRepository,
        IShipmentServiceClient shipmentServiceClient,
        IMapper mapper,
        ILogger<ReleaseCargoCommandHandler> logger)
    {
        _receiptRepository = receiptRepository;
        _binRepository = binRepository;
        _shipmentServiceClient = shipmentServiceClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<CargoReceiptDto>> Handle(ReleaseCargoCommand command, CancellationToken cancellationToken)
    {
        // 1. Find the active receipt for this shipment in this warehouse
        var receipt = await _receiptRepository.GetByShipmentIdAsync(command.Request.ShipmentId)
            ?? throw new NotFoundException("CargoReceipt for shipment", command.Request.ShipmentId);

        if (receipt.WarehouseId != command.Request.WarehouseId)
            throw new UnprocessableException("Shipment is not stored in the specified warehouse.");

        if (receipt.ReleasedAt.HasValue)
            throw new UnprocessableException("Cargo has already been released from this warehouse.");

        // 2. Free the bin
        var bin = await _binRepository.GetByIdAsync(receipt.BinId)
            ?? throw new NotFoundException("Bin", receipt.BinId);

        bin.Release();
        await _binRepository.UpdateAsync(bin);

        // 3. Mark receipt as released
        receipt.MarkReleased();
        await _receiptRepository.UpdateAsync(receipt);

        // 4. Update shipment status to OutForDelivery
        try
        {
            await _shipmentServiceClient.UpdateShipmentStatusAsync(
                command.Request.ShipmentId, "OutForDelivery", command.ActorUserId);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Failed to update shipment {ShipmentId} status to OutForDelivery after release",
                command.Request.ShipmentId);
        }

        var dto = _mapper.Map<CargoReceiptDto>(receipt);
        return ApiResponse<CargoReceiptDto>.Ok(dto, "Cargo released successfully.");
    }
}
