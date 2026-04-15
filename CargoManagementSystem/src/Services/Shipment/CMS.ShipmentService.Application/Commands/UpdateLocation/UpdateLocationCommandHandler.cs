using CMS.ShipmentService.Application.Interfaces;
using CMS.ShipmentService.Domain.Interfaces;
using CMS.ShipmentService.Domain.ValueObjects;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.UpdateLocation;

public class UpdateLocationCommandHandler : IRequestHandler<UpdateLocationCommand, ApiResponse<bool>>
{
    private readonly IShipmentRepository _shipmentRepository;
    private readonly ICacheService _cacheService;

    public UpdateLocationCommandHandler(IShipmentRepository shipmentRepository, ICacheService cacheService)
    {
        _shipmentRepository = shipmentRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<bool>> Handle(UpdateLocationCommand command, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentRepository.GetByIdAsync(command.ShipmentId)
            ?? throw new NotFoundException("Shipment", command.ShipmentId);

        var location = new GpsCoordinate(command.Request.Latitude, command.Request.Longitude, command.Request.RecordedAt);
        shipment.UpdateLocation(location);

        await _shipmentRepository.UpdateAsync(shipment);

        // Invalidate tracking cache so next poll gets fresh location
        await _cacheService.RemoveAsync($"shipment:track:{shipment.TrackingNumber}");

        return ApiResponse<bool>.Ok(true, "Location updated successfully.");
    }
}
