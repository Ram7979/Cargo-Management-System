using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.FleetService.Infrastructure.Persistence.Repositories;

public class GpsHistoryRepository : IGpsHistoryRepository
{
    private readonly FleetDbContext _context;

    public GpsHistoryRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<GpsHistory>> GetByVehicleIdAsync(Guid vehicleId, DateTime? from = null, DateTime? to = null)
    {
        var query = _context.GpsHistories.Where(g => g.VehicleId == vehicleId);
        if (from.HasValue) query = query.Where(g => g.RecordedAt >= from.Value);
        if (to.HasValue) query = query.Where(g => g.RecordedAt <= to.Value);
        return await query.OrderBy(g => g.RecordedAt).ToListAsync();
    }

    public async Task<IEnumerable<GpsHistory>> GetByAssignmentIdAsync(Guid assignmentId)
        => await _context.GpsHistories
            .Where(g => g.AssignmentId == assignmentId)
            .OrderBy(g => g.RecordedAt)
            .ToListAsync();

    public async Task AddAsync(GpsHistory history)
    {
        await _context.GpsHistories.AddAsync(history);
        await _context.SaveChangesAsync();
    }
}
