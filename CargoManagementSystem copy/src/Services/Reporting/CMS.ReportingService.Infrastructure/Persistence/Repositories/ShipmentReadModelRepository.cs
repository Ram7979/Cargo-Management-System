using CMS.ReportingService.Domain.Interfaces;
using CMS.ReportingService.Domain.ReadModels;
using Microsoft.EntityFrameworkCore;

namespace CMS.ReportingService.Infrastructure.Persistence.Repositories;

public class ShipmentReadModelRepository : IShipmentReadModelRepository
{
    private static readonly string[] ActiveStatuses =
    [
        "Pending", "Assigned", "PickedUp", "InTransit", "AtWarehouse",
        "OutForDelivery", "FailedDelivery", "ReturnedToWarehouse"
    ];

    private readonly ReportingDbContext _context;

    public ShipmentReadModelRepository(ReportingDbContext context)
    {
        _context = context;
    }

    public async Task<(IEnumerable<ShipmentReadModel> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null, string? customerCode = null,
        string? origin = null, string? destination = null,
        DateTime? fromDate = null, DateTime? toDate = null,
        string? driverId = null, string? vehicleId = null,
        string? serviceType = null, string? sortBy = null, string? sortDir = null)
    {
        var query = BuildBaseQuery(status, customerCode, origin, destination,
            fromDate, toDate, driverId, vehicleId, serviceType);

        var totalCount = await query.CountAsync();

        // Dynamic sort
        query = ApplySort(query, sortBy, sortDir);

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<ShipmentReadModel>> GetAllFilteredAsync(
        string? status = null, string? customerCode = null,
        DateTime? fromDate = null, DateTime? toDate = null,
        string? origin = null, string? destination = null,
        string? driverId = null, string? vehicleId = null,
        string? serviceType = null)
    {
        var query = BuildBaseQuery(status, customerCode, origin, destination,
            fromDate, toDate, driverId, vehicleId, serviceType);

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<int> CountByStatusAsync(string status, DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.CreatedAt <= toDate.Value);

        return await query.CountAsync();
    }

    public async Task<int> CountActiveAsync()
        => await _context.ShipmentReadModels
            .CountAsync(s => ActiveStatuses.Contains(s.Status));

    public async Task<Dictionary<string, int>> GetStatusCountsAsync(
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (fromDate.HasValue) query = query.Where(s => s.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(s => s.CreatedAt <= toDate.Value);

        return await query
            .GroupBy(s => s.Status)
            .Select(g => new { Status = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.Status, x => x.Count);
    }

    public async Task<Dictionary<string, int>> GetCargoTypeCountsAsync(
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (fromDate.HasValue) query = query.Where(s => s.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(s => s.CreatedAt <= toDate.Value);

        return await query
            .GroupBy(s => s.CargoType)
            .Select(g => new { CargoType = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.CargoType, x => x.Count);
    }

    public async Task<IEnumerable<(string City, string Country, int Count)>> GetVolumeByRegionAsync(
        DateTime? fromDate = null, DateTime? toDate = null)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (fromDate.HasValue) query = query.Where(s => s.CreatedAt >= fromDate.Value);
        if (toDate.HasValue) query = query.Where(s => s.CreatedAt <= toDate.Value);

        var results = await query
            .Where(s => !string.IsNullOrEmpty(s.DestinationCity))
            .GroupBy(s => new { s.DestinationCity, s.DestinationCountry })
            .Select(g => new { g.Key.DestinationCity, g.Key.DestinationCountry, Count = g.Count() })
            .OrderByDescending(x => x.Count)
            .Take(50)
            .ToListAsync();

        return results.Select(r => (r.DestinationCity, r.DestinationCountry, r.Count));
    }

    public async Task<IEnumerable<(DateTime Date, string Status, int Count)>> GetStatusTrendAsync(
        DateTime fromDate, DateTime toDate, string groupBy)
    {
        var query = _context.ShipmentReadModels
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate);

        // Group by day/week/month using EF
        var raw = await query
            .GroupBy(s => new
            {
                Year = s.CreatedAt.Year,
                Month = s.CreatedAt.Month,
                Day = groupBy == "month" ? 1 : (groupBy == "week" ? (s.CreatedAt.Day / 7) * 7 + 1 : s.CreatedAt.Day),
                s.Status
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                g.Key.Status,
                Count = g.Count()
            })
            .ToListAsync();

        return raw.Select(r => (new DateTime(r.Year, r.Month, Math.Min(r.Day, DateTime.DaysInMonth(r.Year, r.Month))), r.Status, r.Count));
    }

    public async Task<IEnumerable<(DateTime Date, decimal Revenue)>> GetRevenueTrendAsync(
        DateTime fromDate, DateTime toDate, string groupBy)
    {
        var query = _context.ShipmentReadModels
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate && s.InvoiceAmount > 0);

        var raw = await query
            .GroupBy(s => new
            {
                Year = s.CreatedAt.Year,
                Month = s.CreatedAt.Month,
                Day = groupBy == "month" ? 1 : (groupBy == "week" ? (s.CreatedAt.Day / 7) * 7 + 1 : s.CreatedAt.Day)
            })
            .Select(g => new
            {
                g.Key.Year,
                g.Key.Month,
                g.Key.Day,
                Revenue = g.Sum(s => s.InvoiceAmount)
            })
            .ToListAsync();

        return raw.Select(r => (new DateTime(r.Year, r.Month, Math.Min(r.Day, DateTime.DaysInMonth(r.Year, r.Month))), r.Revenue));
    }

    public async Task<IEnumerable<(string DriverId, string DriverName, int Deliveries, int OnTime, int Failed)>>
        GetDriverStatsAsync(DateTime fromDate, DateTime toDate, string? driverId = null)
    {
        var query = _context.ShipmentReadModels
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate
                        && s.DriverId != null);

        if (!string.IsNullOrWhiteSpace(driverId))
            query = query.Where(s => s.DriverId == driverId);

        var raw = await query
            .GroupBy(s => new { s.DriverId, s.DriverName })
            .Select(g => new
            {
                g.Key.DriverId,
                DriverName = g.Key.DriverName ?? string.Empty,
                Deliveries = g.Count(s => s.Status == "Delivered"),
                OnTime = g.Count(s => s.Status == "Delivered" && s.DeliveredAt.HasValue),
                Failed = g.Count(s => s.Status == "FailedDelivery")
            })
            .ToListAsync();

        return raw.Select(r => (r.DriverId!, r.DriverName, r.Deliveries, r.OnTime, r.Failed));
    }

    public async Task<IEnumerable<(string CustomerId, string CustomerCode, string CustomerName, decimal TotalInvoiced, decimal TotalPaid, decimal Outstanding)>>
        GetCustomerRevenueAsync(DateTime fromDate, DateTime toDate, string? customerId = null)
    {
        var query = _context.ShipmentReadModels
            .Where(s => s.CreatedAt >= fromDate && s.CreatedAt <= toDate);

        if (!string.IsNullOrWhiteSpace(customerId))
            query = query.Where(s => s.CustomerCode == customerId);

        var raw = await query
            .GroupBy(s => new { s.CustomerCode, s.CustomerName })
            .Select(g => new
            {
                g.Key.CustomerCode,
                g.Key.CustomerName,
                TotalInvoiced = g.Sum(s => s.InvoiceAmount)
            })
            .OrderByDescending(x => x.TotalInvoiced)
            .ToListAsync();

        return raw.Select(r => (r.CustomerCode, r.CustomerCode, r.CustomerName, r.TotalInvoiced, 0m, r.TotalInvoiced));
    }

    public async Task AddOrUpdateAsync(ShipmentReadModel model)
    {
        var existing = await _context.ShipmentReadModels
            .FirstOrDefaultAsync(s => s.TrackingNumber == model.TrackingNumber);

        if (existing is null)
        {
            await _context.ShipmentReadModels.AddAsync(model);
        }
        else
        {
            existing.Status = model.Status;
            existing.CustomerCode = model.CustomerCode;
            existing.CustomerName = model.CustomerName;
            existing.OriginAddress = model.OriginAddress;
            existing.OriginCity = model.OriginCity;
            existing.OriginCountry = model.OriginCountry;
            existing.DestinationAddress = model.DestinationAddress;
            existing.DestinationCity = model.DestinationCity;
            existing.DestinationCountry = model.DestinationCountry;
            existing.WeightKg = model.WeightKg;
            existing.ServiceType = model.ServiceType;
            existing.CargoType = model.CargoType;
            existing.DriverId = model.DriverId;
            existing.DriverName = model.DriverName;
            existing.VehicleId = model.VehicleId;
            existing.PlateNumber = model.PlateNumber;
            existing.InvoiceAmount = model.InvoiceAmount;
            existing.DeliveredAt = model.DeliveredAt;
            existing.PickedUpAt = model.PickedUpAt;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.ShipmentReadModels.Update(existing);
        }

        await _context.SaveChangesAsync();
    }

    // ─── Private helpers ────────────────────────────────────────────────────

    private IQueryable<ShipmentReadModel> BuildBaseQuery(
        string? status, string? customerCode,
        string? origin, string? destination,
        DateTime? fromDate, DateTime? toDate,
        string? driverId, string? vehicleId, string? serviceType)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(customerCode))
            query = query.Where(s => s.CustomerCode == customerCode);

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(s => s.OriginAddress.Contains(origin) ||
                                     s.OriginCity.Contains(origin));

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(s => s.DestinationAddress.Contains(destination) ||
                                     s.DestinationCity.Contains(destination));

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.CreatedAt <= toDate.Value);

        if (!string.IsNullOrWhiteSpace(driverId))
            query = query.Where(s => s.DriverId == driverId);

        if (!string.IsNullOrWhiteSpace(vehicleId))
            query = query.Where(s => s.VehicleId == vehicleId);

        if (!string.IsNullOrWhiteSpace(serviceType))
            query = query.Where(s => s.ServiceType == serviceType);

        return query;
    }

    private static IQueryable<ShipmentReadModel> ApplySort(
        IQueryable<ShipmentReadModel> query, string? sortBy, string? sortDir)
    {
        var desc = sortDir?.Equals("desc", StringComparison.OrdinalIgnoreCase) == true;

        return sortBy?.ToLowerInvariant() switch
        {
            "trackingnumber" => desc ? query.OrderByDescending(s => s.TrackingNumber) : query.OrderBy(s => s.TrackingNumber),
            "customercode" => desc ? query.OrderByDescending(s => s.CustomerCode) : query.OrderBy(s => s.CustomerCode),
            "status" => desc ? query.OrderByDescending(s => s.Status) : query.OrderBy(s => s.Status),
            "weightkg" => desc ? query.OrderByDescending(s => s.WeightKg) : query.OrderBy(s => s.WeightKg),
            "deliveredat" => desc ? query.OrderByDescending(s => s.DeliveredAt) : query.OrderBy(s => s.DeliveredAt),
            "invoiceamount" => desc ? query.OrderByDescending(s => s.InvoiceAmount) : query.OrderBy(s => s.InvoiceAmount),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };
    }
}
