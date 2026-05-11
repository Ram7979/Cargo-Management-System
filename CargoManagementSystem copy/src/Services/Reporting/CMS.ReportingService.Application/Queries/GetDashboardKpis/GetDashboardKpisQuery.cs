using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardKpis;

public record GetDashboardKpisQuery(DateTime? FromDate = null, DateTime? ToDate = null)
    : IRequest<ApiResponse<DashboardKpisDto>>;
