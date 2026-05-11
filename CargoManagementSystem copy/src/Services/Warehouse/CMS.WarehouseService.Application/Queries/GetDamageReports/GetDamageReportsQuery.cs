using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Queries.GetDamageReports;

public record GetDamageReportsQuery(
    Guid? WarehouseId = null,
    string? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null)
    : IRequest<ApiResponse<IEnumerable<DamageReportDto>>>;
