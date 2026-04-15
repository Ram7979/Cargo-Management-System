using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Enums;

namespace CMS.BillingService.Domain.Interfaces;

public interface IInvoiceRepository
{
    Task<Invoice?> GetByIdAsync(Guid id);
    Task<Invoice?> GetByShipmentIdAsync(Guid shipmentId);
    Task<(IEnumerable<Invoice> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        Guid? customerId = null,
        string? status = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        Guid? shipmentId = null);
    Task<IEnumerable<Invoice>> GetOverdueAsync();
    Task AddAsync(Invoice invoice);
    Task UpdateAsync(Invoice invoice);
    Task<bool> ExistsByShipmentIdAsync(Guid shipmentId);
    Task<int> CountAsync();
}
