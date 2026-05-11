using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Commands.AddMaintenanceLog;

public record AddMaintenanceLogCommand(Guid VehicleId, AddMaintenanceLogRequest Request) : IRequest<ApiResponse<MaintenanceLogDto>>;
