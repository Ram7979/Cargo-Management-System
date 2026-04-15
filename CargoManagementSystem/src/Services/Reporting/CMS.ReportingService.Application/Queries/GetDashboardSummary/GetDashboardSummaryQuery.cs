using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardSummary;

public record GetDashboardSummaryQuery : IRequest<ApiResponse<DashboardSummaryDto>>;
