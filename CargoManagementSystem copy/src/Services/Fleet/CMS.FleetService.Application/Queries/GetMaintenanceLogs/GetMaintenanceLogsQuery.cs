using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetMaintenanceLogs;

public record GetMaintenanceLogsQuery(
    Guid VehicleId,
    int Page = 1,
    int PageSize = 20) : IRequest<ApiResponse<IEnumerable<MaintenanceLogDto>>>;
