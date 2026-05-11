using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.UpdateShipmentStatus;

public class UpdateShipmentStatusCommand : IRequest<ApiResponse<ShipmentDto>>
{
    public Guid ShipmentId { get; }
    public UpdateShipmentStatusRequest Request { get; }

    public UpdateShipmentStatusCommand(Guid shipmentId, UpdateShipmentStatusRequest request)
    {
        ShipmentId = shipmentId;
        Request = request;
    }
}
