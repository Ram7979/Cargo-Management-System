using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, ApiResponse<DashboardSummaryDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private readonly IShipmentReadModelRepository _repository;
    private readonly ICacheService _cache;
    private readonly IInvoiceServiceClient _invoiceClient;

    public GetDashboardSummaryQueryHandler(
        IShipmentReadModelRepository repository,
        ICacheService cache,
        IInvoiceServiceClient invoiceClient)
    {
        _repository = repository;
        _cache = cache;
        _invoiceClient = invoiceClient;
    }

    public async Task<ApiResponse<DashboardSummaryDto>> Handle(
        GetDashboardSummaryQuery request,
        CancellationToken cancellationToken)
    {
        // Cache key incorporates date range so different ranges don't collide
        var fromStr = request.FromDate?.ToString("yyyyMMdd") ?? "all";
        var toStr = request.ToDate?.ToString("yyyyMMdd") ?? "all";
        var cacheKey = $"dashboard:summary:{fromStr}:{toStr}";

        var cached = await _cache.GetAsync<DashboardSummaryDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<DashboardSummaryDto>.Ok(cached);

        var today = DateTime.UtcNow.Date;
        var monthStart = new DateTime(today.Year, today.Month, 1, 0, 0, 0, DateTimeKind.Utc);

        // Run DB counts sequentially to avoid DbContext concurrency issues
        var statusCounts = await _repository.GetStatusCountsAsync(request.FromDate, request.ToDate);
        var totalActive = await _repository.CountActiveAsync();
        var totalToday = await _repository.CountByStatusAsync(string.Empty, today, today.AddDays(1).AddTicks(-1));
        var deliveredToday = await _repository.CountByStatusAsync("Delivered", today, today.AddDays(1).AddTicks(-1));

        // External service calls can still be parallel
        var pendingInvoicesTask = _invoiceClient.GetPendingInvoicesCountAsync();
        var revenueTask = _invoiceClient.GetRevenueAsync(monthStart, today.AddDays(1).AddTicks(-1));

        await Task.WhenAll(pendingInvoicesTask, revenueTask);

        var pendingInvoices = await pendingInvoicesTask;
        var revenueThisMonth = await revenueTask;

        statusCounts.TryGetValue("Pending", out var pendingPickups);
        statusCounts.TryGetValue("InTransit", out var inTransit);
        statusCounts.TryGetValue("FailedDelivery", out var failedDeliveries);

        var dto = new DashboardSummaryDto
        {
            TotalActiveShipments = totalActive,
            ShipmentsByStatus = statusCounts,
            PendingInvoicesCount = pendingInvoices,
            TotalShipmentsToday = totalToday,
            DeliveredToday = deliveredToday,
            PendingPickups = pendingPickups,
            InTransitCount = inTransit,
            FailedDeliveries = failedDeliveries,
            RevenueThisMonth = revenueThisMonth,
            FromDate = request.FromDate,
            ToDate = request.ToDate
        };

        await _cache.SetAsync(cacheKey, dto, CacheTtl);

        return ApiResponse<DashboardSummaryDto>.Ok(dto);
    }
}
