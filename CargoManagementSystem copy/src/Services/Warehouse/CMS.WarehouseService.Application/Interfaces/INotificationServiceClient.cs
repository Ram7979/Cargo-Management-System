namespace CMS.WarehouseService.Application.Interfaces;

public interface INotificationServiceClient
{
    Task SendWarehouseArrivalNotificationAsync(string customerId, Guid shipmentId, string trackingNumber, string warehouseName);
}
