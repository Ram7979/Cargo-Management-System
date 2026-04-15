using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Commands.UpdateLocation;

public record UpdateLocationCommand(Guid ShipmentId, UpdateLocationRequest Request) : IRequest<ApiResponse<bool>>;
