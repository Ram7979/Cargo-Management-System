using CMS.ReportingService.Domain.ReadModels;

namespace CMS.ReportingService.Application.Interfaces;

public interface IShipmentServiceClient
{
    Task<IEnumerable<ShipmentReadModel>> GetRecentShipmentsAsync(int page = 1, int pageSize = 100);
}
