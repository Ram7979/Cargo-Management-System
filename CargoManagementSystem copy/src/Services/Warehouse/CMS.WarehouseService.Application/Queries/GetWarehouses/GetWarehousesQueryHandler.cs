using AutoMapper;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetWarehouses;

public class GetWarehousesQueryHandler : IRequestHandler<GetWarehousesQuery, ApiResponse<IEnumerable<WarehouseDto>>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public GetWarehousesQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<WarehouseDto>>> Handle(GetWarehousesQuery request, CancellationToken cancellationToken)
    {
        var warehouses = await _warehouseRepository.GetAllAsync(request.City, request.Country);

        if (request.HasAvailableBins.HasValue)
        {
            warehouses = request.HasAvailableBins.Value
                ? warehouses.Where(w => w.Bins.Any(b => !b.IsOccupied && b.IsActive))
                : warehouses.Where(w => !w.Bins.Any(b => !b.IsOccupied && b.IsActive));
        }

        var dtos = _mapper.Map<IEnumerable<WarehouseDto>>(warehouses);
        return ApiResponse<IEnumerable<WarehouseDto>>.Ok(dtos);
    }
}
