using AutoMapper;
using CMS.ShipmentService.Application.DTOs;
using CMS.ShipmentService.Application.Events;
using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.ShipmentService.Domain.StateMachine;
using CMS.ShipmentService.Domain.ValueObjects;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.UpdateShipmentStatus;

public class UpdateShipmentStatusCommandHandler : IRequestHandler<UpdateShipmentStatusCommand, ApiResponse<ShipmentDto>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICacheService _cacheService;
    private readonly ICurrentUserService _currentUserService;
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    public UpdateShipmentStatusCommandHandler(
        IShipmentRepository shipmentRepository,
        ICacheService cacheService,
        ICurrentUserService currentUserService,
        IMediator mediator,
        IMapper mapper)
    {
        _shipmentRepository = shipmentRepository;
        _cacheService = cacheService;
        _currentUserService = currentUserService;
        _mediator = mediator;
        _mapper = mapper;
    }

    public async Task<ApiResponse<ShipmentDto>> Handle(UpdateShipmentStatusCommand command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId)
            ?? throw new NotFoundException("Shipment", command.ShipmentId);

        var normalizedStatus = command.Request.Status?.Replace("_", "").Replace(" ", "");
        if (!Enum.TryParse<ShipmentStatus>(normalizedStatus, ignoreCase: true, out var targetStatus))
        {
            var allowedValues = string.Join(", ", Enum.GetNames<ShipmentStatus>().Select(n => n.ToUpper()));
            throw new ValidationException(new[] { $"'{command.Request.Status}' is not a valid shipment status. Allowed values: {allowedValues}" });
        }

        if (!ShipmentStateMachine.CanTransition(shipment.Status, targetStatus))
            throw new UnprocessableException($"Cannot transition from {shipment.Status} to {targetStatus}.");

        // POD required for Delivered
        if (targetStatus == ShipmentStatus.Delivered)
        {
            if (string.IsNullOrWhiteSpace(command.Request.PodImageUrl) &&
                string.IsNullOrWhiteSpace(command.Request.PodSignatureData))
                throw new ValidationException(new[] { "Proof of delivery (PodImageUrl or PodSignatureData) is required when transitioning to Delivered." });
        }

        // Failure reason required for FailedDelivery
        if (targetStatus == ShipmentStatus.FailedDelivery)
        {
            if (string.IsNullOrWhiteSpace(command.Request.FailureReason))
                throw new ValidationException(new[] { "FailureReason is required when transitioning to FailedDelivery." });

            if (!Enum.TryParse<FailureReason>(command.Request.FailureReason, ignoreCase: true, out var failureReason))
                throw new ValidationException(new[] { $"Invalid FailureReason. Valid values: {string.Join(", ", Enum.GetNames<FailureReason>())}" });

            shipment.SetFailureReason(failureReason, command.Request.ReDeliveryScheduledAt);
        }

        // Build GPS coordinate if provided
        GpsCoordinate? location = null;
        if (command.Request.GpsLatitude.HasValue && command.Request.GpsLongitude.HasValue)
            location = new GpsCoordinate(command.Request.GpsLatitude.Value, command.Request.GpsLongitude.Value, DateTime.UtcNow);

        var actorId = _currentUserService.UserId;
        var oldStatus = shipment.Status.ToString();

        shipment.UpdateStatus(targetStatus, actorId, command.Request.Notes, location);

        if (!string.IsNullOrWhiteSpace(command.Request.PodImageUrl) || !string.IsNullOrWhiteSpace(command.Request.PodSignatureData))
            shipment.SetPod(command.Request.PodImageUrl ?? string.Empty, command.Request.PodSignatureData ?? string.Empty);

        await _shipmentRepository.UpdateAsync(shipment);

        await _cacheService.RemoveAsync($"shipment:{shipment.Id}");
        await _cacheService.RemoveAsync($"shipment:track:{shipment.TrackingNumber}");

        await _mediator.Publish(new ShipmentStatusChangedEvent(
            shipment.Id, shipment.TrackingNumber, shipment.CustomerId,
            oldStatus, targetStatus.ToString()), cancellationToken);

        var dto = _mapper.Map<ShipmentDto>(shipment);
        return ApiResponse<ShipmentDto>.Ok(dto, "Shipment status updated successfully.");
    }
}
