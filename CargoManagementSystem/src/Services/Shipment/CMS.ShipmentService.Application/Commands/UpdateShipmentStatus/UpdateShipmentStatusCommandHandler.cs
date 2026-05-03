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
        try
        {
            var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId)
                ?? throw new NotFoundException("Shipment", command.ShipmentId);

            var normalizedStatus = command.Request.Status?.Replace("_", "").Replace(" ", "").ToLowerInvariant();
            if (!Enum.GetValues<ShipmentStatus>().Any(v => v.ToString().ToLowerInvariant() == normalizedStatus))
            {
                var allowedValues = string.Join(", ", Enum.GetNames<ShipmentStatus>());
                return ApiResponse<ShipmentDto>.Fail($"'{command.Request.Status}' is not a valid status. Choose from: {allowedValues}");
            }
            var targetStatus = Enum.GetValues<ShipmentStatus>().First(v => v.ToString().ToLowerInvariant() == normalizedStatus);

            if (!ShipmentStateMachine.CanTransition(shipment.Status, targetStatus))
            {
                Console.WriteLine($"[DEBUG] Cannot transition from {shipment.Status} to {targetStatus}");
                throw new UnprocessableException($"Cannot transition from {shipment.Status} to {targetStatus}.");
            }

            // POD optional but recommended for Delivered
            if (targetStatus == ShipmentStatus.Delivered)
            {
                if (string.IsNullOrWhiteSpace(command.Request.PodImageUrl) &&
                    string.IsNullOrWhiteSpace(command.Request.PodSignatureData))
                {
                    Console.WriteLine("[DEBUG] No POD provided for Delivered status. Proceeding anyway.");
                }
            }

            // Failure reason optional for FailedDelivery
            if (targetStatus == ShipmentStatus.FailedDelivery)
            {
                if (!string.IsNullOrWhiteSpace(command.Request.FailureReason))
                {
                    if (Enum.TryParse<FailureReason>(command.Request.FailureReason, ignoreCase: true, out var failureReason))
                    {
                        shipment.SetFailureReason(failureReason, command.Request.ReDeliveryScheduledAt);
                    }
                }
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

            // Non-critical operations - don't let these fail the response
            try { await _cacheService.RemoveAsync($"shipment:{shipment.Id}"); } catch { }
            try { await _cacheService.RemoveAsync($"shipment:track:{shipment.TrackingNumber}"); } catch { }

            try
            {
                await _mediator.Publish(new ShipmentStatusChangedEvent(
                    shipment.Id, shipment.TrackingNumber, shipment.CustomerId,
                    oldStatus, targetStatus.ToString()), cancellationToken);
            }
            catch (Exception pubEx)
            {
                Console.WriteLine($"[WARN] Event publish failed (non-critical): {pubEx.Message}");
            }

            ShipmentDto? dto = null;
            try { dto = _mapper.Map<ShipmentDto>(shipment); } catch { }

            return ApiResponse<ShipmentDto>.Ok(dto, "Shipment status updated successfully.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[ERROR] Failed to handle UpdateShipmentStatusCommand: {ex.Message}");
            return ApiResponse<ShipmentDto>.Fail(ex.Message);
        }
    }
}
