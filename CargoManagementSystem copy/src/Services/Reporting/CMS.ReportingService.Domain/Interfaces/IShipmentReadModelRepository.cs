using CMS.ReportingService.Domain.ReadModels;

namespace CMS.ReportingService.Domain.Interfaces;

public interface IShipmentReadModelRepository
{
    Task<(IEnumerable<ShipmentReadModel> Items, int TotalCount)> GetPagedAsync(
        int page,
        int pageSize,
        string? status = null,
        string? customerCode = null,
        string? origin = null,
        string? destination = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? driverId = null,
        string? vehicleId = null,
        string? serviceType = null,
        string? sortBy = null,
        string? sortDir = null);

    Task<IEnumerable<ShipmentReadModel>> GetAllFilteredAsync(
        string? status = null,
        string? customerCode = null,
        DateTime? fromDate = null,
        DateTime? toDate = null,
        string? origin = null,
        string? destination = null,
        string? driverId = null,
        string? vehicleId = null,
        string? serviceType = null);

    Task<int> CountByStatusAsync(string status, DateTime? fromDate = null, DateTime? toDate = null);

    Task<int> CountActiveAsync();

    Task<Dictionary<string, int>> GetStatusCountsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    Task<Dictionary<string, int>> GetCargoTypeCountsAsync(DateTime? fromDate = null, DateTime? toDate = null);

    Task<IEnumerable<(string City, string Country, int Count)>> GetVolumeByRegionAsync(DateTime? fromDate = null, DateTime? toDate = null);

    Task<IEnumerable<(DateTime Date, string Status, int Count)>> GetStatusTrendAsync(DateTime fromDate, DateTime toDate, string groupBy);

    Task<IEnumerable<(DateTime Date, decimal Revenue)>> GetRevenueTrendAsync(DateTime fromDate, DateTime toDate, string groupBy);

    Task<IEnumerable<(string DriverId, string DriverName, int Deliveries, int OnTime, int Failed)>> GetDriverStatsAsync(
        DateTime fromDate, DateTime toDate, string? driverId = null);

    Task<IEnumerable<(string CustomerId, string CustomerCode, string CustomerName, decimal TotalInvoiced, decimal TotalPaid, decimal Outstanding)>>
        GetCustomerRevenueAsync(DateTime fromDate, DateTime toDate, string? customerId = null);

    Task AddOrUpdateAsync(ShipmentReadModel model);
}
