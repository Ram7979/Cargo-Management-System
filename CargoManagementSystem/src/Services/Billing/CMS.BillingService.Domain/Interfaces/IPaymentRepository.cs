using CMS.BillingService.Domain.Entities;

namespace CMS.BillingService.Domain.Interfaces;

public interface IPaymentRepository
{
    Task<Payment?> GetByIdAsync(Guid id);
    Task<IEnumerable<Payment>> GetByInvoiceIdAsync(Guid invoiceId);
    Task<(IEnumerable<Payment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Guid? invoiceId = null,
        string? method = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
    Task AddAsync(Payment payment);
}
