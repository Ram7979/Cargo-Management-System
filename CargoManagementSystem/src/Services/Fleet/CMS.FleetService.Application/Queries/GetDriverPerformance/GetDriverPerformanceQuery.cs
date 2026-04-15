using CMS.FleetService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.FleetService.Application.Queries.GetDriverPerformance;

public record GetDriverPerformanceQuery(Guid DriverId, DateTime? DateFrom, DateTime? DateTo) : IRequest<ApiResponse<DriverPerformanceDto>>;
