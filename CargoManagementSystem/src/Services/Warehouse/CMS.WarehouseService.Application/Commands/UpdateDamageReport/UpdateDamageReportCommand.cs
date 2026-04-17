using CMS.Shared.Responses;
using CMS.WarehouseService.Application.DTOs;
using MediatR;

namespace CMS.WarehouseService.Application.Commands.UpdateDamageReport;

public record UpdateDamageReportCommand(Guid ReportId, UpdateDamageReportRequest Request) : IRequest<ApiResponse<DamageReportDto>>;
