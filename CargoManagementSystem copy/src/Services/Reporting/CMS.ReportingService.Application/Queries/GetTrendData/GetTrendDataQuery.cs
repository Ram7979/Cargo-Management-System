using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetTrendData;

public record GetTrendDataQuery(
    DateTime FromDate,
    DateTime ToDate,
    string GroupBy = "day") // "day", "week", "month"
    : IRequest<ApiResponse<TrendDataDto>>;
