using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetVehicleRoute;

public class GetVehicleRouteQueryHandler : IRequestHandler<GetVehicleRouteQuery, ApiResponse<IEnumerable<GpsHistoryDto>>>
{
    private readonly IGpsHistoryRepository _gpsHistoryRepository;

    public GetVehicleRouteQueryHandler(IGpsHistoryRepository gpsHistoryRepository)
    {
        _gpsHistoryRepository = gpsHistoryRepository;
    }

    public async Task<ApiResponse<IEnumerable<GpsHistoryDto>>> Handle(GetVehicleRouteQuery request, CancellationToken cancellationToken)
    {
        IEnumerable<Domain.Entities.GpsHistory> history;

        if (request.AssignmentId.HasValue)
            history = await _gpsHistoryRepository.GetByAssignmentIdAsync(request.AssignmentId.Value);
        else
            history = await _gpsHistoryRepository.GetByVehicleIdAsync(request.VehicleId);

        var dtos = history.Select(h => new GpsHistoryDto
        {
            Latitude = h.Latitude,
            Longitude = h.Longitude,
            RecordedAt = h.RecordedAt
        });

        return ApiResponse<IEnumerable<GpsHistoryDto>>.Ok(dtos);
    }
}
