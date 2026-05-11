using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.CancelShipment;

public class CancelShipmentCommandHandler : IRequestHandler<CancelShipmentCommand, ApiResponse<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICacheService _cacheService;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public CancelShipmentCommandHandler(
        IShipmentRepository shipmentRepository,
        ICacheService cacheService,
        IMediator mediator,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _cacheService = cacheService;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ShipmentDto>> Handle(CancelShipmentCommand command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId)
            ?? throw new NotFoundException("Shipment", command.ShipmentId);

        // Only Pending or Assigned can be cancelled directly
        if (shipment.Status != ShipmentStatus.Pending && shipment.Status != ShipmentStatus.Assigned)
            throw new UnprocessableException(
                $"Shipment in status '{shipment.Status}' cannot be cancelled. Only Pending or Assigned shipments can be cancelled.");

        shipment.SetCancellationReason(command.Request.CancellationReason);
        shipment.UpdateStatus(ShipmentStatus.Cancelled, command.ActorId, $"Cancelled: {command.Request.CancellationReason}");

        await _shipmentRepository.UpdateAsync(shipment);

        await _cacheService.RemoveAsync($"shipment:{shipment.Id}");
        await _cacheService.RemoveAsync($"shipment:track:{shipment.TrackingNumber}");

        await _mediator.Publish(new ShipmentStatusChangedEvent(
            shipment.Id, shipment.TrackingNumber, shipment.CustomerId,
            "Active", "Cancelled"), cancellationToken);

        var dto = _mapper.Map<ShipmentDto>(shipment);
        return ApiResponse<ShipmentDto>.Ok(dto, "Shipment cancelled successfully.");
    }
}
