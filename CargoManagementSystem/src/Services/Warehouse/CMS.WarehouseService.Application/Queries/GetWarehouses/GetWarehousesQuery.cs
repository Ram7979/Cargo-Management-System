using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetWarehouses;

public record GetWarehousesQuery(string? City = null, string? Country = null, bool? HasAvailableBins = null)
    : IRequest<ApiResponse<IEnumerable<WarehouseDto>>>;
