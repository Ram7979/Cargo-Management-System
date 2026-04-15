using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.AddVehicle;

public record AddVehicleCommand(AddVehicleRequest Request) : IRequest<ApiResponse<VehicleDto>>;
