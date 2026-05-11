using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetTrendData;

public class GetTrendDataQueryHandler
    : IRequestHandler<GetTrendDataQuery, ApiResponse<TrendDataDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(5);

    private readonly IShipmentReadModelRepository _repository;
    private readonly ICacheService _cache;
    private readonly IInvoiceServiceClient _invoiceClient;

    public GetTrendDataQueryHandler(
        IShipmentReadModelRepository repository, 
        ICacheService cache,
        IInvoiceServiceClient invoiceClient)
    {
        _repository = repository;
        _cache = cache;
        _invoiceClient = invoiceClient;
    }

    public async Task<ApiResponse<TrendDataDto>> Handle(
        GetTrendDataQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"dashboard:trend:{request.FromDate:yyyyMMdd}:{request.ToDate:yyyyMMdd}:{request.GroupBy}";

        var cached = await _cache.GetAsync<TrendDataDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<TrendDataDto>.Ok(cached);

        var revenueTrendTask = _invoiceClient.GetRevenueTrendAsync(request.FromDate, request.ToDate);
        var statusTrendTask = _repository.GetStatusTrendAsync(request.FromDate, request.ToDate, request.GroupBy);
        var cargoTypeTask = _repository.GetCargoTypeCountsAsync(request.FromDate, request.ToDate);
        var geoTask = _repository.GetVolumeByRegionAsync(request.FromDate, request.ToDate);

        await Task.WhenAll(revenueTrendTask, statusTrendTask, cargoTypeTask, geoTask);

        var revenueTrend = (await revenueTrendTask).ToList();

        var statusTrend = (await statusTrendTask).Select(s => new StatusTrendPointDto
        {
            Status = s.Status,
            Count = s.Count
        }).ToList();

        var cargoTypeCounts = await cargoTypeTask;
        var totalCargo = cargoTypeCounts.Values.Sum();
        var cargoBreakdown = cargoTypeCounts.Select(kv => new CargoTypeBreakdownDto
        {
            CargoType = kv.Key,
            Count = kv.Value,
            Percentage = totalCargo > 0 ? Math.Round((double)kv.Value / totalCargo * 100, 2) : 0
        }).ToList();

        var geoVolume = (await geoTask).Select(g => new GeographicVolumeDto
        {
            City = g.City,
            Country = g.Country,
            ShipmentCount = g.Count
        }).ToList();

        var dto = new TrendDataDto
        {
            RevenueTrend = revenueTrend,
            ShipmentsByStatus = statusTrend,
            CargoTypeBreakdown = cargoBreakdown,
            GeographicVolume = geoVolume
        };

        await _cache.SetAsync(cacheKey, dto, CacheTtl);

        return ApiResponse<TrendDataDto>.Ok(dto);
    }
}
