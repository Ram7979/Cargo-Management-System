using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Enums;
using CMS.BillingService.Domain.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace CMS.BillingService.Infrastructure.Persistence.Repositories;

public class PaymentRepository : IPaymentRepository
{
    private readonly BillingDbContext _context;

    public PaymentRepository(BillingDbContext context)
    {
        _context = context;
    }

    public async Task<Payment?> GetByIdAsync(Guid id)
        => await _context.Payments.FirstOrDefaultAsync(p => p.Id == id);

    public async Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId)
        => await _context.Payments.Where(p => p.InvoiceId == invoiceId)
            .OrderByDescending(p => p.PaidAt).ToListAsync();

    public async Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Guid? invoiceId = null,
        string? method = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null)
    {
        var query = _context.Payments.AsQueryable();

        if (invoiceId.HasValue) query = query.Where(p => p.InvoiceId == invoiceId.Value);
        if (!string.IsNullOrWhiteSpace(method) && Enum.TryParse<PaymentMethod>(method, ignoreCase: true, out var m))
            query = query.Where(p => p.Method == m);
        if (dateFrom.HasValue) query = query.Where(p => p.PaidAt >= dateFrom.Value);
        if (dateTo.HasValue) query = query.Where(p => p.PaidAt <= dateTo.Value);

        var totalCount = await query.CountAsync();
        var items = await query.OrderByDescending(p => p.PaidAt)
            .Skip((page - 1) * pageSize).Take(pageSize).ToListAsync();

        return (items, totalCount);
    }

    public async Task AddAsync(Payment payment)
    {
        await _context.Payments.AddAsync(payment);
        await _context.SaveChangesAsync();
    }
}
