using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Exceptions;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetVehicle;

public class GetVehicleQueryHandler : IRequestHandler<GetVehicleQuery, ApiResponse<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehicleQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<VehicleDto>> Handle(GetVehicleQuery request, CancellationToken cancellationToken)
    {
        var vehicle = await _vehicleRepository.GetByIdAsync(request.VehicleId)
            ?? throw new VehicleNotFoundException($"Vehicle '{request.VehicleId}' not found.");

        var dto = _mapper.Map<VehicleDto>(vehicle);
        return ApiResponse<VehicleDto>.Ok(dto);
    }
}
