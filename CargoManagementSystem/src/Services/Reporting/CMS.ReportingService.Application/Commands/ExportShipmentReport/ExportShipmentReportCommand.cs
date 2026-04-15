using CMS.ReportingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Commands.ExportShipmentReport;

public record ExportShipmentReportCommand(
    ShipmentReportFilter Filter,
    string Format) : IRequest<ApiResponse<string>>;
