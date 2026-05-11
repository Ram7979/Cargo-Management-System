using CMS.ReportingService.Application.DTOs;

namespace CMS.ReportingService.Application.Interfaces;

public interface IInvoiceServiceClient
{
    Task<int> GetPendingInvoicesCountAsync();
    Task<decimal> GetRevenueAsync(DateTime fromDate, DateTime toDate);
    Task<IEnumerable<RevenueTrendPointDto>> GetRevenueTrendAsync(DateTime fromDate, DateTime toDate);
    Task<(decimal TotalInvoiced, decimal TotalCollected, decimal Outstanding)> GetRevenueSummaryAsync(
        DateTime fromDate, DateTime toDate, string? customerId = null, string? serviceType = null);
}
