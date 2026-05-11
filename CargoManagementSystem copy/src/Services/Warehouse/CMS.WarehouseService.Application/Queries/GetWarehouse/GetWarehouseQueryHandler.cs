using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetWarehouse;

public class GetWarehouseQueryHandler : IRequestHandler<GetWarehouseQuery, ApiResponse<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public GetWarehouseQueryHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WarehouseDto>> Handle(GetWarehouseQuery request, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(request.WarehouseId)
            ?? throw new NotFoundException("Warehouse", request.WarehouseId);

        var dto = _mapper.Map<WarehouseDto>(warehouse);
        return ApiResponse<WarehouseDto>.Ok(dto);
    }
}
