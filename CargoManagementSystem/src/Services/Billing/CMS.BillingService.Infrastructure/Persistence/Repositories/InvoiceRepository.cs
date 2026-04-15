using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Enums;
using CMS.BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.BillingService.Infrastructure.Persistence.Repositories;

public class InvoiceRepository : IInvoiceRepository
{
    private readonly BillingDbContext _context;

    public InvoiceRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<Invoice?> GetByIdAsync(Guid id)
        => await _context.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.Id == id);

    public async Task<Invoice?> GetByShipmentIdAsync(Guid shipmentId)
        => await _context.Invoices.Include(i => i.Payments).FirstOrDefaultAsync(i => i.ShipmentId == shipmentId);

    public async Task<(IEnumerable<Invoice> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Guid? customerId = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        Guid? shipmentId = null)
    {
        var query = _context.Invoices.AsQueryable();

        if (customerId.HasValue) query = query.Where(i => i.CustomerId == customerId.Value);
        if (shipmentId.HasValue) query = query.Where(i => i.ShipmentId == shipmentId.Value);
        if (!string.IsNullOrWhiteSpace(status) && Enum.TryParse<InvoiceStatus>(status, ignoreCase: true, out var s))
            query = query.Where(i => i.Status == s);
        if (dateFrom.HasValue) query = query.Where(i => i.CreatedAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(i => i.CreatedAt <= dateTo.Value);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(i => i.CreatedAt)
            .Skip((page - 1) * pageSize).Take(pageSize)
            .Include(i => i.Payments).ToListAsync();

        return (items, totalCount);
    }

    public async Task<IEnumerable<Invoice>> GetOverdueAsync()
    {
        var now = DateTime.UtcNow;
        return await _context.Invoices
            .Where(i => i.DueDate.HasValue && i.DueDate.Value < now
                && i.Status != InvoiceStatus.Paid && i.Status != InvoiceStatus.Void)
            .Include(i => i.Payments)
            .ToListAsync();
    }

    public async Task AddAsync(Invoice invoice)
    {
        await _context.Invoices.AddAsync(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(Invoice invoice)
    {
        _context.Invoices.Update(invoice);
        await _context.SaveChangesAsync();
    }

    public async Task<bool> ExistsByShipmentIdAsync(Guid shipmentId)
        => await _context.Invoices.AnyAsync(i => i.ShipmentId == shipmentId);

    public async Task<int> CountAsync()
        => await _context.Invoices.CountAsync();
}
