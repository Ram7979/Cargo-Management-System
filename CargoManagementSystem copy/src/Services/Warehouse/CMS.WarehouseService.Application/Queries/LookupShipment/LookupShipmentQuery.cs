using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.LookupShipment;

public record LookupShipmentQuery(string TrackingNumber) : IRequest<ApiResponse<ShipmentLookupDto>>;
