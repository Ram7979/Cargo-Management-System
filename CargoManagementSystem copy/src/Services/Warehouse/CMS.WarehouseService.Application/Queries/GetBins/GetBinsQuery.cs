using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetBins;

public record GetBinsQuery(
    Guid WarehouseId,
    bool? Available = null,
    string? Zone = null,
    string? Level = null)
    : IRequest<ApiResponse<IEnumerable<BinDto>>>;
