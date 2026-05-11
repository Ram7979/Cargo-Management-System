using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateWarehouse;

public class UpdateWarehouseCommandHandler : IRequestHandler<UpdateWarehouseCommand, ApiResponse<WarehouseDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IMapper _mapper;

    public UpdateWarehouseCommandHandler(IWarehouseRepository warehouseRepository, IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<WarehouseDto>> Handle(UpdateWarehouseCommand command, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(command.WarehouseId)
            ?? throw new NotFoundException("Warehouse", command.WarehouseId);

        warehouse.Update(
            command.Request.Name,
            command.Request.Address,
            command.Request.City,
            command.Request.Country,
            command.Request.CapacityKg);

        await _warehouseRepository.UpdateAsync(warehouse);

        var dto = _mapper.Map<WarehouseDto>(warehouse);
        return ApiResponse<WarehouseDto>.Ok(dto, "Warehouse updated successfully.");
    }
}
