using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.AddBin;

public class AddBinCommandHandler : IRequestHandler<AddBinCommand, ApiResponse<BinDto>>
{
    private readonly IWarehouseRepository _warehouseRepository;
    private readonly IBinRepository _binRepository;
    private readonly IMapper _mapper;

    public AddBinCommandHandler(
        IWarehouseRepository warehouseRepository,
        IBinRepository binRepository,
        IMapper mapper)
    {
        _warehouseRepository = warehouseRepository;
        _binRepository = binRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BinDto>> Handle(AddBinCommand command, CancellationToken cancellationToken)
    {
        var warehouse = await _warehouseRepository.GetByIdAsync(command.WarehouseId)
            ?? throw new NotFoundException("Warehouse", command.WarehouseId);

        // Check BinCode uniqueness within warehouse
        var existingBins = await _binRepository.GetAllByWarehouseAsync(command.WarehouseId);
        if (existingBins.Any(b => b.BinCode.Equals(command.Request.BinCode, StringComparison.OrdinalIgnoreCase)))
            throw new ConflictException($"Bin with code '{command.Request.BinCode}' already exists in this warehouse.");

        var bin = Bin.Create(
            command.WarehouseId,
            command.Request.BinCode,
            command.Request.CapacityKg,
            command.Request.Zone,
            command.Request.Level);

        await _binRepository.AddAsync(bin);

        var dto = _mapper.Map<BinDto>(bin);
        return ApiResponse<BinDto>.Ok(dto, "Bin added successfully.");
    }
}
