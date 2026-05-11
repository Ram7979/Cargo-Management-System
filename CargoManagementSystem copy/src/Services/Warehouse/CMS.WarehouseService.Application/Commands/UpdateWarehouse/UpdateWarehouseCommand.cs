using CMS.WarehouseService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateWarehouse;

public record UpdateWarehouseCommand(Guid WarehouseId, UpdateWarehouseRequest Request) : IRequest<ApiResponse<WarehouseDto>>;
