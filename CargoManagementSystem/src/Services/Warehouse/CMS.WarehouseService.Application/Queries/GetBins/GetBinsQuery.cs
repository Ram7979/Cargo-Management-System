using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetBins;

public record GetBinsQuery(Guid WarehouseId) : IRequest<ApiResponse<IEnumerable<BinDto>>>;
