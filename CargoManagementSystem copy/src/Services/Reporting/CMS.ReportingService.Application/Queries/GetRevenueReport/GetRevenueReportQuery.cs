using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetRevenueReport;

public record GetRevenueReportQuery(
    DateTime FromDate,
    DateTime ToDate,
    string? CustomerId = null,
    string? ServiceType = null,
    string? GroupBy = null)
    : IRequest<ApiResponse<RevenueReportDto>>;
