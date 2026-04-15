using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetAvailableDrivers;

public class GetAvailableDriversQueryHandler : IRequestHandler<GetAvailableDriversQuery, ApiResponse<IEnumerable<DriverDto>>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public GetAvailableDriversQueryHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<DriverDto>>> Handle(GetAvailableDriversQuery request, CancellationToken cancellationToken)
    {
        var drivers = await _driverRepository.GetAvailableAsync();
        var dtos = _mapper.Map<IEnumerable<DriverDto>>(drivers);
        return ApiResponse<IEnumerable<DriverDto>>.Ok(dtos);
    }
}
