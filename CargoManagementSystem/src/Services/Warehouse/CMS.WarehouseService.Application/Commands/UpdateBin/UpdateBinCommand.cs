using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateBin;

public record UpdateBinCommand(Guid WarehouseId, Guid BinId, UpdateBinRequest Request) : IRequest<ApiResponse<BinDto>>;
