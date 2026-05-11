using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipment;

public class GetShipmentQuery : IRequest<ApiResponse<ShipmentDto>>
{
    public Guid ShipmentId { get; }

    public GetShipmentQuery(Guid shipmentId)
    {
        ShipmentId = shipmentId;
    }
}
