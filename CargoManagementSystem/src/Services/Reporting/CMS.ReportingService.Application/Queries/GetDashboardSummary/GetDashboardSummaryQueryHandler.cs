using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetDashboardSummary;

public class GetDashboardSummaryQueryHandler
    : IRequestHandler<GetDashboardSummaryQuery, ApiResponse<DashboardSummaryDto>>
{
    private const string CacheKey = "dashboard:summary";
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private static readonly string[] AllStatuses =
    [
        "Pending", "Assigned", "PickedUp", "InTransit", "AtWarehouse",
        "OutForDelivery", "Delivered", "FailedDelivery", "ReturnedToWarehouse", "Cancelled"
    ];

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
        var cached = await _cache.GetAsync<DashboardSummaryDto>(CacheKey);
        if (cached is not null)
            return ApiResponse<DashboardSummaryDto>.Ok(cached);

        var shipmentsByStatus = new Dictionary<string, int>();
        foreach (var status in AllStatuses)
        {
            var count = await _repository.CountByStatusAsync(status);
            shipmentsByStatus[status] = count;
        }

        var totalActive = await _repository.CountActiveAsync();
        var pendingInvoices = await _invoiceClient.GetPendingInvoicesCountAsync();

        var today = DateTime.UtcNow.Date;
        var (todayItems, todayCount) = await _repository.GetPagedAsync(
            1, int.MaxValue, null, null, null, null, today, today.AddDays(1).AddTicks(-1));

        var dto = new DashboardSummaryDto
        {
            TotalActiveShipments = totalActive,
            ShipmentsByStatus = shipmentsByStatus,
            PendingInvoicesCount = pendingInvoices,
            TotalShipmentsToday = todayCount
        };

        await _cache.SetAsync(CacheKey, dto, CacheTtl);

        return ApiResponse<DashboardSummaryDto>.Ok(dto);
    }
}
