using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetWarehouse;

public record GetWarehouseQuery(Guid WarehouseId) : IRequest<ApiResponse<WarehouseDto>>;
