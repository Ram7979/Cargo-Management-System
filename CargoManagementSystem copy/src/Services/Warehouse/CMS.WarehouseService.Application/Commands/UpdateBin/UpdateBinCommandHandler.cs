using AutoMapper;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using CMS.WarehouseService.Domain.Interfaces;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateBin;

public class UpdateBinCommandHandler : IRequestHandler<UpdateBinCommand, ApiResponse<BinDto>>
{
    private readonly IBinRepository _binRepository;
    private readonly IMapper _mapper;

    public UpdateBinCommandHandler(IBinRepository binRepository, IMapper mapper)
    {
        _binRepository = binRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<BinDto>> Handle(UpdateBinCommand command, CancellationToken cancellationToken)
    {
        var bin = await _binRepository.GetByIdAsync(command.BinId)
            ?? throw new NotFoundException("Bin", command.BinId);

        if (bin.WarehouseId != command.WarehouseId)
            throw new NotFoundException("Bin", command.BinId);

        // Cannot update capacity of an occupied bin
        if (command.Request.CapacityKg.HasValue && bin.IsOccupied)
            throw new UnprocessableException("Cannot update capacity of an occupied bin.");

        bin.Update(
            command.Request.BinCode,
            command.Request.Zone,
            command.Request.Level,
            command.Request.CapacityKg,
            command.Request.IsActive);

        await _binRepository.UpdateAsync(bin);

        var dto = _mapper.Map<BinDto>(bin);
        return ApiResponse<BinDto>.Ok(dto, "Bin updated successfully.");
    }
}
