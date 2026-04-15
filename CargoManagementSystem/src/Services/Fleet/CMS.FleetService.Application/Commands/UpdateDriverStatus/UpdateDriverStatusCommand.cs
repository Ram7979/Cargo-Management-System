using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.UpdateDriverStatus;

public record UpdateDriverStatusCommand(Guid DriverId, UpdateDriverStatusRequest Request) : IRequest<ApiResponse<DriverDto>>;
