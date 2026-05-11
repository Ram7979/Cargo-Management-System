using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetFleetReport;

public record GetFleetReportQuery(
    DateTime FromDate,
    DateTime ToDate,
    string? DriverId = null,
    string? VehicleId = null)
    : IRequest<ApiResponse<FleetReportDto>>;
