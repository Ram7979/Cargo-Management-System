using CMS.ReportingService.Application.DTOs;
using CMS.ReportingService.Application.Interfaces;
using CMS.ReportingService.Domain.Interfaces;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.ReportingService.Application.Queries.GetTrendData;

public class GetTrendDataQueryHandler
    : IRequestHandler<GetTrendDataQuery, ApiResponse<TrendDataDto>>
{
    private static readonly TimeSpan CacheTtl = TimeSpan.FromSeconds(60);

    private readonly IShipmentReadModelRepository _repository;
    private readonly ICacheService _cache;

    public GetTrendDataQueryHandler(IShipmentReadModelRepository repository, ICacheService cache)
    {
        _repository = repository;
        _cache = cache;
    }

    public async Task<ApiResponse<TrendDataDto>> Handle(
        GetTrendDataQuery request,
        CancellationToken cancellationToken)
    {
        var cacheKey = $"dashboard:trend:{request.FromDate:yyyyMMdd}:{request.ToDate:yyyyMMdd}:{request.GroupBy}";

        var cached = await _cache.GetAsync<TrendDataDto>(cacheKey);
        if (cached is not null)
            return ApiResponse<TrendDataDto>.Ok(cached);

        var revenueTrendTask = _repository.GetRevenueTrendAsync(request.FromDate, request.ToDate, request.GroupBy);
        var statusTrendTask = _repository.GetStatusTrendAsync(request.FromDate, request.ToDate, request.GroupBy);
        var cargoTypeTask = _repository.GetCargoTypeCountsAsync(request.FromDate, request.ToDate);
        var geoTask = _repository.GetVolumeByRegionAsync(request.FromDate, request.ToDate);

        await Task.WhenAll(revenueTrendTask, statusTrendTask, cargoTypeTask, geoTask);

        var revenueTrend = (await revenueTrendTask).Select(r => new RevenueTrendPointDto
        {
            Date = r.Date.ToString(request.GroupBy == "month" ? "yyyy-MM" : "yyyy-MM-dd"),
            Value = r.Revenue
        }).ToList();

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
