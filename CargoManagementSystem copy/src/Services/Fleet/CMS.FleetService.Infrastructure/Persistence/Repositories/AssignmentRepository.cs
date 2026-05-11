using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.FleetService.Infrastructure.Persistence.Repositories;

public class AssignmentRepository : IAssignmentRepository
{
    private readonly FleetDbContext _context;

    public AssignmentRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<Assignment?> GetByIdAsync(Guid id)
        => await _context.Assignments.FirstOrDefaultAsync(a => a.Id == id);

    public async Task<Assignment?> GetActiveByDriverIdAsync(Guid driverId)
        => await _context.Assignments
            .FirstOrDefaultAsync(a => a.DriverId == driverId && a.Status == AssignmentStatus.Active);

    public async Task<Assignment?> GetActiveByVehicleIdAsync(Guid vehicleId)
        => await _context.Assignments
            .FirstOrDefaultAsync(a => a.VehicleId == vehicleId && a.Status == AssignmentStatus.Active);

    public async Task<Assignment?> GetByShipmentIdAsync(Guid shipmentId)
        => await _context.Assignments.FirstOrDefaultAsync(a => a.ShipmentId == shipmentId);

    public async Task<(IEnumerable<Assignment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        Guid? driverId = null,
        Guid? vehicleId = null,
        Guid? shipmentId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.Assignments.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<AssignmentStatus>(status, ignoreCase: true, out var as_))
            query = query.Where(a => a.Status == as_);

        if (driverId.HasValue) query = query.Where(a => a.DriverId == driverId.Value);
        if (vehicleId.HasValue) query = query.Where(a => a.VehicleId == vehicleId.Value);
        if (shipmentId.HasValue) query = query.Where(a => a.ShipmentId == shipmentId.Value);
        if (dateFrom.HasValue) query = query.Where(a => a.AssignedAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(a => a.AssignedAt <= dateTo.Value);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(a => a.AssignedAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Assignment assignment)
    {
        await _context.Assignments.AddAsync(assignment);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Assignment assignment)
    {
        _context.Assignments.Update(assignment);
        await _context.SaveChangesAsync();
    }
}
