using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetVehicles;

public class GetVehiclesQueryHandler : IRequestHandler<GetVehiclesQuery, PagedResponse<VehicleDto>>
{
    private readonly IVehicleRepository _vehicleRepository;
    private readonly IMapper _mapper;

    public GetVehiclesQueryHandler(IVehicleRepository vehicleRepository, IMapper mapper)
    {
        _vehicleRepository = vehicleRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<VehicleDto>> Handle(GetVehiclesQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _vehicleRepository.GetPagedAsync(
            request.Page, request.PageSize, request.Status, request.Type);
        var dtos = _mapper.Map<IEnumerable<VehicleDto>>(items);
        return PagedResponse<VehicleDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
