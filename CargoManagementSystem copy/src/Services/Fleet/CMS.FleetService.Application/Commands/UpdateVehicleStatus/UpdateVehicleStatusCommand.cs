using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateVehicleStatus;

public record UpdateVehicleStatusCommand(Guid VehicleId, UpdateVehicleStatusRequest Request) : IRequest<ApiResponse<VehicleDto>>;
