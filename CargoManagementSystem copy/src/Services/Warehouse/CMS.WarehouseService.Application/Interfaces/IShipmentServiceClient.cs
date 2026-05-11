using CMS.WarehouseService.Application.DTOs;

namespace CMS.WarehouseService.Application.Interfaces;

public interface IShipmentServiceClient
{
    Task<string> GetShipmentStatusAsync(Guid shipmentId);
    Task<ShipmentLookupDto?> GetShipmentByTrackingNumberAsync(string trackingNumber);
    Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId);
}
