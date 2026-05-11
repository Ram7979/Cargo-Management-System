using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetMaintenanceLogs;

public class GetMaintenanceLogsQueryHandler
    : IRequestHandler<GetMaintenanceLogsQuery, ApiResponse<IEnumerable<MaintenanceLogDto>>>
{
    private readonly IMaintenanceLogRepository _maintenanceLogRepository;
    private readonly IMapper _mapper;

    public GetMaintenanceLogsQueryHandler(
        IMaintenanceLogRepository maintenanceLogRepository,
        IMapper mapper)
    {
        _maintenanceLogRepository = maintenanceLogRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<MaintenanceLogDto>>> Handle(
        GetMaintenanceLogsQuery request,
        CancellationToken cancellationToken)
    {
        var logs = await _maintenanceLogRepository.GetByVehicleIdAsync(request.VehicleId);

        var page = request.Page < 1 ? 1 : request.Page;
        var pageSize = request.PageSize < 1 ? 20 : request.PageSize > 100 ? 100 : request.PageSize;

        var paged = logs
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToList();

        var dtos = _mapper.Map<IEnumerable<MaintenanceLogDto>>(paged);
        return ApiResponse<IEnumerable<MaintenanceLogDto>>.Ok(dtos);
    }
}
