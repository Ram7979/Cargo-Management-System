namespace CMS.FleetService.Application.Interfaces;

public interface IShipmentServiceClient
{
    Task UpdateShipmentStatusAsync(Guid shipmentId, string status, string actorId);
}
