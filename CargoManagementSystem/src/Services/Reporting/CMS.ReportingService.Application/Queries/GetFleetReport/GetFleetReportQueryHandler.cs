using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetFleetReport;

public class GetFleetReportQueryHandler
    : IRequestHandler<GetFleetReportQuery, ApiResponse<FleetReportDto>>
{
    private readonly IShipmentReadModelRepository _repository;

    public GetFleetReportQueryHandler(IShipmentReadModelRepository repository)
    {
        _repository = repository;
    }

    public async Task<ApiResponse<FleetReportDto>> Handle(
        GetFleetReportQuery request,
        CancellationToken cancellationToken)
    {
        var driverStats = await _repository.GetDriverStatsAsync(
            request.FromDate, request.ToDate, request.DriverId);

        var driverDtos = driverStats.Select(d => new DriverStatsDto
        {
            DriverId = d.DriverId,
            DriverName = d.DriverName,
            TotalDeliveries = d.Deliveries,
            OnTimeDeliveries = d.OnTime,
            FailedDeliveries = d.Failed,
            OnTimeRatePercent = d.Deliveries > 0
                ? Math.Round((double)d.OnTime / d.Deliveries * 100, 2) : 0,
            FailedRatePercent = d.Deliveries > 0
                ? Math.Round((double)d.Failed / d.Deliveries * 100, 2) : 0
        }).ToList();

        var dto = new FleetReportDto
        {
            DriverStats = driverDtos,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        return ApiResponse<FleetReportDto>.Ok(dto);
    }
}
