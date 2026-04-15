using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.FleetService.Infrastructure.Persistence.Repositories;

public class MaintenanceLogRepository : IMaintenanceLogRepository
{
    private readonly FleetDbContext _context;

    public MaintenanceLogRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<MaintenanceLog>> GetByVehicleIdAsync(Guid vehicleId)
        => await _context.MaintenanceLogs
            .Where(m => m.VehicleId == vehicleId)
            .OrderByDescending(m => m.ServiceDate)
            .ToListAsync();

    public async Task AddAsync(MaintenanceLog log)
    {
        await _context.MaintenanceLogs.AddAsync(log);
        await _context.SaveChangesAsync();
    }
}
