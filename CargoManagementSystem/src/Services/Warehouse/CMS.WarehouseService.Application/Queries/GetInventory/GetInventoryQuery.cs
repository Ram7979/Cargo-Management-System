using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetInventory;

public record GetInventoryQuery(Guid WarehouseId) : IRequest<ApiResponse<InventorySummaryDto>>;
