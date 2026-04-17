using AutoMapper;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.CreateWarehouse;

public class CreateWarehouseCommandHandler : IRequestHandler<CreateWarehouseCommand, ApiResponse<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public CreateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WarehouseDto>> Handle(CreateWarehouseCommand command, CancellationToken cancellationToken)
    {
        var warehouse = Warehouse.Create(
            command.Request.Name,
            command.Request.Address,
            command.Request.City,
            command.Request.Country,
            command.Request.CapacityKg,
            command.Request.TotalBins);

        await _warehouseRepository.AddAsync(warehouse);

        var dto = _mapper.Map<WarehouseDto>(warehouse);
        return ApiResponse<WarehouseDto>.Ok(dto, "Warehouse created successfully.");
    }
}
