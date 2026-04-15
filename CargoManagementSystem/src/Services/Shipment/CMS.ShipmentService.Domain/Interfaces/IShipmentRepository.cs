using CMS.ShipmentService.Domain.Entities;

namespace CMS.ShipmentService.Domain.Interfaces;

public interface IShipmentRepository
{
    Task<Shipment?> GetByIdAsync(Guid id);
    Task<Shipment?> GetByTrackingNumberAsync(string trackingNumber);
    Task<(IEnumerable<Shipment> Items, int TotalCount)> GetPagedAsync(
        int page, int pageSize,
        string? status = null,
        Guid? customerId = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null,
        string? serviceType = null,
        string? trackingNumberSearch = null,
        string? sortBy = null,
        string? sortDir = null);
    Task AddAsync(Shipment shipment);
    Task UpdateAsync(Shipment shipment);
    Task<int> GetNextSequenceAsync(int year);
}
