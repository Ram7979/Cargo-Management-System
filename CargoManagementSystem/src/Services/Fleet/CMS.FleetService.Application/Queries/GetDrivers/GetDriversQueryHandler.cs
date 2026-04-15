using AutoMapper;
using CMS.FleetService.Application.DTOs;
using CMS.FleetService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDrivers;

public class GetDriversQueryHandler : IRequestHandler<GetDriversQuery, PagedResponse<DriverDto>>
{
    private readonly IDriverRepository _driverRepository;
    private readonly IMapper _mapper;

    public GetDriversQueryHandler(IDriverRepository driverRepository, IMapper mapper)
    {
        _driverRepository = driverRepository;
        _mapper = mapper;
    }

    public async Task<PagedResponse<DriverDto>> Handle(GetDriversQuery request, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _driverRepository.GetPagedAsync(request.Page, request.PageSize, request.Status);
        var dtos = _mapper.Map<IEnumerable<DriverDto>>(items);
        return PagedResponse<DriverDto>.Ok(dtos, request.Page, request.PageSize, totalCount);
    }
}
