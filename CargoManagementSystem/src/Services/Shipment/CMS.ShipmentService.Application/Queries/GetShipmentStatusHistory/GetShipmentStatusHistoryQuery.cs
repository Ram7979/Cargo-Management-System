using CMS.ShipmentService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ShipmentService.Application.Queries.GetShipmentStatusHistory;

public record GetShipmentStatusHistoryQuery(Guid ShipmentId)
    : IRequest<ApiResponse<IEnumerable<ShipmentStatusHistoryDto>>>;
