using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAvailableVehicles;

public class GetAvailableVehiclesQueryHandler : IRequestHandler<GetAvailableVehiclesQuery, ApiResponse<IEnumerable<VehicleDto>>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetAvailableVehiclesQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<VehicleDto>>> Handle(GetAvailableVehiclesQuery request, CancellationToken cancellationToken)
    {
        var vehicles = await _vehicleRepository.GetAvailableAsync(request.MinCapacityKg, request.Type);
        var dtos = _mapper.Map<IEnumerable<VehicleDto>>(vehicles);
        return ApiResponse<IEnumerable<VehicleDto>>.Ok(dtos);
    }
}
