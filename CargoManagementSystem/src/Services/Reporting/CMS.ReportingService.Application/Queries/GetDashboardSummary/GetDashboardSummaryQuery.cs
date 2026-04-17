using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery(DateTime? FromDate = null, DateTime? ToDate = null)
    : IRequest<ApiResponse<DashboardSummaryDto>>;
