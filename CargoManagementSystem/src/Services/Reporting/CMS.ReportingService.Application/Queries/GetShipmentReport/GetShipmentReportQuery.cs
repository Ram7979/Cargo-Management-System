using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetShipmentReport;

public record GetShipmentReportQuery(
    int Page,
    int PageSize,
    ShipmentReportFilter Filter) : IRequest<PagedResponse<ShipmentReportDto>>;
