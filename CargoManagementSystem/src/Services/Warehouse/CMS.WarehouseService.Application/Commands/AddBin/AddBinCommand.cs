using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.AddBin;

public record AddBinCommand(Guid WarehouseId, AddBinRequest Request) : IRequest<ApiResponse<BinDto>>;
