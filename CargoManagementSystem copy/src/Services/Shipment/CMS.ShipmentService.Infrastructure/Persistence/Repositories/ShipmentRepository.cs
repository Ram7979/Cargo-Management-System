using CMS.ShipmentService.Domain.Entities;
using CMS.ShipmentService.Domain.Enums;
using CMS.ShipmentService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.ShipmentService.Infrastructure.Persistence.Repositories;

public class ShipmentRepository : IShipmentRepository
{
    private readonly ShipmentDbContext _context;

    public ShipmentRepository(ShipmentDbContext context)
    {
        _context = context;
    }

    public async Task<Shipment?> GetByIdAsync(Guid id)
    {
        return await _context.Shipments
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber)
    {
        return await _context.Shipments
            .Include(s => s.StatusHistory)
            .FirstOrDefaultAsync(s => s.TrackingNumber == trackingNumber);
    }

    public async Task<(IEnumerable<Shipment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        string? customerId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? serviceType = null,
        string? trackingNumberSearch = null,
        string? sortBy = null,
        string? sortDir = null)
    {
        var query = _context.Shipments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<ShipmentStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(s => s.Status == parsedStatus);

        if (!string.IsNullOrWhiteSpace(customerId))
            query = query.Where(s => s.CustomerId == customerId);

        if (dateFrom.HasValue)
            query = query.Where(s => s.CreatedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(s => s.CreatedAt <= dateTo.Value);

        if (!string.IsNullOrWhiteSpace(serviceType))
            query = query.Where(s => s.ServiceType.ToLower() == serviceType.ToLower());

        if (!string.IsNullOrWhiteSpace(trackingNumberSearch))
            query = query.Where(s => s.TrackingNumber.Contains(trackingNumberSearch));

        var totalCount = await query.CountAsync();

        // Sorting
        query = (sortBy?.ToLower(), sortDir?.ToLower()) switch
        {
            ("createdat", "asc") => query.OrderBy(s => s.CreatedAt),
            ("createdat", _) => query.OrderByDescending(s => s.CreatedAt),
            ("weight", "asc") => query.OrderBy(s => s.WeightKg),
            ("weight", _) => query.OrderByDescending(s => s.WeightKg),
            _ => query.OrderByDescending(s => s.CreatedAt)
        };

        var items = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Shipment shipment)
    {
        await _context.Shipments.AddAsync(shipment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Shipment shipment)
    {
        // Ensure the shipment entity is marked as modified
        var entry = _context.Entry(shipment);
        if (entry.State == EntityState.Detached)
        {
            _context.Shipments.Attach(shipment);
        }
        entry.State = EntityState.Modified;

        // Ensure new StatusHistory entries are marked as Added (not Modified)
        foreach (var historyEntry in shipment.StatusHistory)
        {
            var historyState = _context.Entry(historyEntry);
            if (historyState.State == EntityState.Modified && 
                !await _context.ShipmentStatusHistories.AnyAsync(h => h.Id == historyEntry.Id))
            {
                historyState.State = EntityState.Added;
            }
        }

        await _context.SaveChangesAsync();
    }

    public async Task<int> GetNextSequenceAsync(int year)
    {
        // Use MAX sequence to avoid race conditions
        var maxSeq = await _context.Shipments
            .Where(s => s.CreatedAt.Year == year)
            .CountAsync();
        return maxSeq + 1;
    }
}
