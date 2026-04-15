using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriver;

public class GetDriverQueryHandler : IRequestHandler<GetDriverQuery, ApiResponse<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public GetDriverQueryHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<DriverDto>> Handle(GetDriverQuery request, CancellationToken cancellationToken)
    {
        var driver = await _driverRepository.GetByIdAsync(request.DriverId)
            ?? throw new NotFoundException("Driver", request.DriverId);

        var dto = _mapper.Map<DriverDto>(driver);
        return ApiResponse<DriverDto>.Ok(dto);
    }
}
