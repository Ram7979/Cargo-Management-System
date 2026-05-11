namespace CMS.ShipmentService.Application.Interfaces;

public interface ICustomerServiceClient
{
    Task<bool> CustomerExistsAsync(string customerId);
}
