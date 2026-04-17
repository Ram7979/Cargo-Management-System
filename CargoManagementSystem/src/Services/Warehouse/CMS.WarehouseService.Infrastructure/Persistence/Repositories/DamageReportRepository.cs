using CMS.WarehouseService.Domain.Entities;
using CMS.WarehouseService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.WarehouseService.Infrastructure.Persistence.Repositories;

public class DamageReportRepository : IDamageReportRepository
{
    private readonly WarehouseDbContext _context;

    public DamageReportRepository(WarehouseDbContext context)
    {
        _context = context;
    }

    public async Task<DamageReport?> GetByIdAsync(Guid id)
        => await _context.DamageReports.FirstOrDefaultAsync(d => d.Id == id);

    public async Task<IEnumerable<DamageReport>> GetAllAsync(
        Guid? warehouseId = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.DamageReports.AsQueryable();

        if (warehouseId.HasValue)
            query = query.Where(d => d.WarehouseId == warehouseId.Value);

        if (!string.IsNullOrWhiteSpace(status) &&
            Enum.TryParse<DamageReportStatus>(status, ignoreCase: true, out var parsedStatus))
            query = query.Where(d => d.Status == parsedStatus);

        if (dateFrom.HasValue)
            query = query.Where(d => d.ReportedAt >= dateFrom.Value);

        if (dateTo.HasValue)
            query = query.Where(d => d.ReportedAt <= dateTo.Value);

        return await query.OrderByDescending(d => d.ReportedAt).ToListAsync();
    }

    public async Task AddAsync(DamageReport report)
    {
        await _context.DamageReports.AddAsync(report);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(DamageReport report)
    {
        _context.DamageReports.Update(report);
        await _context.SaveChangesAsync();
    }
}
