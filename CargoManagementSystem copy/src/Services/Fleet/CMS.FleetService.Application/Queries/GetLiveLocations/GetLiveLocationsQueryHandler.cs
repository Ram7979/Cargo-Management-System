using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Application.Interfaces;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetLiveLocations;

public class GetLiveLocationsQueryHandler : IRequestHandler<GetLiveLocationsQuery, ApiResponse<IEnumerable<VehicleDto>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public GetLiveLocationsQueryHandler(
        IVehicleRepository vehicleRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<VehicleDto>>> Handle(GetLiveLocationsQuery request, CancellationToken cancellationToken)
    {
        // Try Redis first, fall back to DB
        var cachedLocations = await _cacheService.GetByPatternAsync<GpsCoordinateDto>("fleet:live:*");
        if (cachedLocations.Any())
        {
            // Return vehicles with live locations from DB, enriched with cached GPS
            var vehicles = await _vehicleRepository.GetAvailableAsync();
            var dtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
            return ApiResponse<IEnumerable<VehicleDto>>.Ok(dtos);
        }

        // Fall back to DB query for all vehicles with a last location
        var allVehicles = await _vehicleRepository.GetAllAsync();
        var vehiclesWithLocation = allVehicles.Where(v => v.LastLocation != null);
        var result = _mapper.Map<IEnumerable<VehicleDto>>(vehiclesWithLocation);
        return ApiResponse<IEnumerable<VehicleDto>>.Ok(result);
    }
}
