using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.SubmitGpsUpdate;

public record SubmitGpsUpdateCommand(Guid VehicleId, Guid DriverId, SubmitGpsUpdateRequest Request) : IRequest<ApiResponse<bool>>;
