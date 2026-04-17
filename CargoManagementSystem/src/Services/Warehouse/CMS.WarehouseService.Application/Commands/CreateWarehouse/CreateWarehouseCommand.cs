using CMS.WarehouseService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.CreateWarehouse;

public record CreateWarehouseCommand(CreateWarehouseRequest Request) : IRequest<ApiResponse<WarehouseDto>>;
