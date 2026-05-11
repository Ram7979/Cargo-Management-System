using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateVehicle;

public record UpdateVehicleCommand(Guid VehicleId, UpdateVehicleRequest Request) : IRequest<ApiResponse<VehicleDto>>;
