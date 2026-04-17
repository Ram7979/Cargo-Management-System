using AutoMapper;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetBins;

public class GetBinsQueryHandler : IRequestHandler<GetBinsQuery, ApiResponse<IEnumerable<BinDto>>>
{
    private readonly IBinRepository _binRepository;
    private readonly IMapper _mapper;

    public GetBinsQueryHandler(IBinRepository binRepository, IMapper mapper)
    {
        _binRepository = binRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<BinDto>>> Handle(GetBinsQuery request, CancellationToken cancellationToken)
    {
        var bins = await _binRepository.GetAllByWarehouseAsync(
            request.WarehouseId,
            request.Available,
            request.Zone,
            request.Level);

        var dtos = _mapper.Map<IEnumerable<BinDto>>(bins);
        return ApiResponse<IEnumerable<BinDto>>.Ok(dtos);
    }
}
