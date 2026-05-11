using CMS.FleetService.Domain.Entities;
using CMS.FleetService.Domain.Enums;
using CMS.FleetService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.FleetService.Infrastructure.Persistence.Repositories;

public class VehicleRepository : IVehicleRepository
{
    private readonly FleetDbContext _context;

    public VehicleRepository(FleetDbContext context)
    {
        _context = context;
    }

    public async Task<Vehicle?> GetByIdAsync(Guid id)
        => await _context.Vehicles.Include(v => v.MaintenanceLogs).FirstOrDefaultAsync(v => v.Id == id);

    public async Task<Vehicle?> GetByPlateNumberAsync(string plateNumber)
        => await _context.Vehicles.FirstOrDefaultAsync(v => v.PlateNumber == plateNumber);

    public async Task<(IEnumerable<Vehicle> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize, string? status = null, string? type = null)
    {
        var query = _context.Vehicles.AsQueryable();

        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<VehicleStatus>(status, ignoreCase: true, out var vs))
            query = query.Where(v => v.Status == vs);

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<VehicleType>(type, ignoreCase: true, out var vt))
            query = query.Where(v => v.Type == vt);

        var totalCount = await query.CountAsync();
        var items = await query.OrderBy(v => v.PlateNumber)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Vehicle>> GetAllAsync()
        => await _context.Vehicles.OrderBy(v => v.PlateNumber).ToListAsync();

    public async Task<IEnumerable<Vehicle>> GetAvailableAsync(decimal? minCapacityKg = null, string? type = null)
    {
        var query = _context.Vehicles.Where(v => v.Status == VehicleStatus.Available);

        if (minCapacityKg.HasValue)
            query = query.Where(v => v.CapacityKg - v.CurrentLoadKg >= minCapacityKg.Value);

        if (!string.IsNullOrWhiteSpace(type) && Enum.TryParse<VehicleType>(type, ignoreCase: true, out var vt))
            query = query.Where(v => v.Type == vt);

        return await query.ToListAsync();
    }

    public async Task AddAsync(Vehicle vehicle)
    {
        await _context.Vehicles.AddAsync(vehicle);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Vehicle vehicle)
    {
        _context.Vehicles.Update(vehicle);
        await _context.SaveChangesAsync();
    }
}
