using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.CancelShipment;

public record CancelShipmentCommand(Guid ShipmentId, CancelShipmentRequest Request, string ActorId) : IRequest<ApiResponse<ShipmentDto>>;
