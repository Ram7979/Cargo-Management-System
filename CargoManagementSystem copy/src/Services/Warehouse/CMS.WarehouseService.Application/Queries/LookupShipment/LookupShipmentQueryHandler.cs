using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Application.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.LookupShipment;

public class LookupShipmentQueryHandler : IRequestHandler<LookupShipmentQuery, ApiResponse<ShipmentLookupDto>>
{
    private readonly IShipmentServiceClient _shipmentServiceClient;

    public LookupShipmentQueryHandler(IShipmentServiceClient shipmentServiceClient)
    {
        _shipmentServiceClient = shipmentServiceClient;
    }

    public async Task<ApiResponse<ShipmentLookupDto>> Handle(LookupShipmentQuery request, CancellationToken cancellationToken)
    {
        var shipment = await _shipmentServiceClient.GetShipmentByTrackingNumberAsync(request.TrackingNumber)
            ?? throw new NotFoundException("Shipment", request.TrackingNumber);

        return ApiResponse<ShipmentLookupDto>.Ok(shipment);
    }
}
