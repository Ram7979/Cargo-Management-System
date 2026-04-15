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
        int page,
        int pageSize,
        string? status,
        string? customerCode,
        string? origin,
        string? destination,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(customerCode))
            query = query.Where(s => s.CustomerCode == customerCode);

        if (!string.IsNullOrWhiteSpace(origin))
            query = query.Where(s => s.OriginAddress.Contains(origin));

        if (!string.IsNullOrWhiteSpace(destination))
            query = query.Where(s => s.DestinationAddress.Contains(destination));

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.CreatedAt <= toDate.Value);

        var totalCount = await query.CountAsync();

        var items = await query
            .OrderByDescending(s => s.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<ShipmentReadModel>> GetAllFilteredAsync(
        string? status,
        string? customerCode,
        DateTime? fromDate,
        DateTime? toDate)
    {
        var query = _context.ShipmentReadModels.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status))
            query = query.Where(s => s.Status == status);

        if (!string.IsNullOrWhiteSpace(customerCode))
            query = query.Where(s => s.CustomerCode == customerCode);

        if (fromDate.HasValue)
            query = query.Where(s => s.CreatedAt >= fromDate.Value);

        if (toDate.HasValue)
            query = query.Where(s => s.CreatedAt <= toDate.Value);

        return await query.OrderByDescending(s => s.CreatedAt).ToListAsync();
    }

    public async Task<int> CountByStatusAsync(string status)
    {
        return await _context.ShipmentReadModels.CountAsync(s => s.Status == status);
    }

    public async Task<int> CountActiveAsync()
    {
        return await _context.ShipmentReadModels
            .CountAsync(s => ActiveStatuses.Contains(s.Status));
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
            existing.DestinationAddress = model.DestinationAddress;
            existing.WeightKg = model.WeightKg;
            existing.ServiceType = model.ServiceType;
            existing.DeliveredAt = model.DeliveredAt;
            existing.UpdatedAt = DateTime.UtcNow;
            _context.ShipmentReadModels.Update(existing);
        }

        await _context.SaveChangesAsync();
    }
}
