using CMS.ReportingService.Domain.ReadModels;

namespace CMS.ReportingService.Domain.Interfaces;

public interface IShipmentReadModelRepository
{
    Task<(IEnumerable<ShipmentReadModel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status,
        string? customerCode,
        string? origin,
        string? destination,
        DateTime? fromDate,
        DateTime? toDate);

    Task<IEnumerable<ShipmentReadModel>> GetAllFilteredAsync(
        string? status,
        string? customerCode,
        DateTime? fromDate,
        DateTime? toDate);

    Task<int> CountByStatusAsync(string status);

    Task<int> CountActiveAsync();

    Task AddOrUpdateAsync(ShipmentReadModel model);
}
