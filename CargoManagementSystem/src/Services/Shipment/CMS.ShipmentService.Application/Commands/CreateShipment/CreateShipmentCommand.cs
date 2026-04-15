using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.CreateShipment;

public class CreateShipmentCommand : IRequest<ApiResponse<ShipmentDto>>
{
    public CreateShipmentRequest Request { get; }

    public CreateShipmentCommand(CreateShipmentRequest request)
    {
        Request = request;
    }
}
