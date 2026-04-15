namespace CMS.WarehouseService.Application.Interfaces;

public interface IShipmentServiceClient
{
    Task<string> GetShipmentStatusAsync(Guid shipmentId);
    Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId);
}
