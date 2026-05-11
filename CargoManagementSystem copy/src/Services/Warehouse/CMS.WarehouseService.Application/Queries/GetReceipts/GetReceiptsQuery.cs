using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetReceipts;

public record GetReceiptsQuery(
    int Page,
    int PageSize,
    Guid? WarehouseId = null,
    Guid? ShipmentId = null,
    bool? HasDamage = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null)
    : IRequest<ApiResponse<PagedResponse<CargoReceiptDto>>>;
