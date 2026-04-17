using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardKpis;

public class GetDashboardKpisQueryHandler
    : IRequestHandler<GetDashboardKpisQuery, ApiResponse<IEnumerable<DashboardKpiDto>>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

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

    public async Task<ApiResponse<IEnumerable<DashboardKpiDto>>> Handle(
        GetDashboardKpisQuery request,
        CancellationToken cancellationToken)
    {
        var fromStr = request.FromDate?.ToString("yyyyMMdd") ?? "all";
        var toStr = request.ToDate?.ToString("yyyyMMdd") ?? "all";
        var cacheKey = $"dashboard:kpis:{fromStr}:{toStr}";

        var cached = await _cache.GetAsync<IEnumerable<DashboardKpiDto>>(cacheKey);
        if (cached is not null)
            return ApiResponse<IEnumerable<DashboardKpiDto>>.Ok(cached);

        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        var statusCountsTask = _repository.GetStatusCountsAsync(request.FromDate, request.ToDate);
        var deliveredTodayTask = _repository.CountByStatusAsync("Delivered", today, today.AddDays(1).AddTicks(-1));
        var revenueTask = _invoiceClient.GetRevenueAsync(monthStart, today.AddDays(1).AddTicks(-1));

        await Task.WhenAll(statusCountsTask, deliveredTodayTask, revenueTask);

        var statusCounts = await statusCountsTask;
        var deliveredToday = await deliveredTodayTask;
        var revenue = await revenueTask;

        statusCounts.TryGetValue("Pending", out var pendingPickups);
        statusCounts.TryGetValue("InTransit", out var inTransit);
        statusCounts.TryGetValue("FailedDelivery", out var failedDeliveries);

        var kpis = new List<DashboardKpiDto>
        {
            new() { Label = "Delivered Today",    Value = deliveredToday,  Unit = "shipments", Trend = "flat" },
            new() { Label = "Pending Pickups",    Value = pendingPickups,  Unit = "shipments", Trend = "flat" },
            new() { Label = "In Transit",         Value = inTransit,       Unit = "shipments", Trend = "flat" },
            new() { Label = "Failed Deliveries",  Value = failedDeliveries, Unit = "shipments", Trend = "flat" },
            new() { Label = "Revenue This Month", Value = revenue,         Unit = "USD",       Trend = "flat" }
        };

        await _cache.SetAsync(cacheKey, kpis, CacheTtl);

        return ApiResponse<IEnumerable<DashboardKpiDto>>.Ok(kpis);
    }
}
