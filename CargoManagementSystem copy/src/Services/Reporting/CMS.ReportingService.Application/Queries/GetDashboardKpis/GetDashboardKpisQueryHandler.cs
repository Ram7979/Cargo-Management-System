using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardKpis;

public class GetDashboardKpisQueryHandler
    : IRequestHandler<GetDashboardKpisQuery, ApiResponse<DashboardKpisDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly IShipmentReadModelRepository _repository;
    private readonly ICacheService _cache;
    private readonly IInvoiceServiceClient _invoiceClient;

    public GetDashboardKpisQueryHandler(
        IShipmentReadModelRepository repository,
        ICacheService cache,
        IInvoiceServiceClient invoiceClient)
    {
        _repository = repository;
        _cache = cache;
        _invoiceClient = invoiceClient;
    }

    public async Task<ApiResponse<DashboardKpisDto>> Handle(
        GetDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"dashboard:kpis:v2:{request.FromDate:yyyyMMdd}:{request.ToDate:yyyyMMdd}";

        var cached = await _cache.GetAsync<DashboardKpisDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<DashboardKpisDto>.Ok(cached);

        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var statusCountsTask = _repository.GetStatusCountsAsync(request.FromDate, request.ToDate);
        var revenueTask = _invoiceClient.GetRevenueAsync(monthStart, today.AddDays(1).AddTicks(-1));

        await Task.WhenAll(statusCountsTask, revenueTask);

        var statusCounts = await statusCountsTask;
        var revenue = await revenueTask;

        statusCounts.TryGetValue("InTransit", out var inTransit);
        statusCounts.TryGetValue("Pending", out var pending);

        var dto = new DashboardKpisDto
        {
            TotalRevenue = revenue,
            ActiveShipments = inTransit + pending,
            WarehouseUtilization = 72.5, // Mocked for now
            FleetAvailability = 88.0      // Mocked for now
        };

        await _cache.SetAsync(cacheKey, dto, CacheTtl);

        return ApiResponse<DashboardKpisDto>.Ok(dto);
    }
}
