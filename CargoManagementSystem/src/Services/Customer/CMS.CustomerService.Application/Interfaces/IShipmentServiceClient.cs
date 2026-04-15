using CMS.CustomerService.Application.DTOs;

namespace CMS.CustomerService.Application.Interfaces;

public interface IShipmentServiceClient
{
    Task<(IEnumerable<CustomerShipmentDto> Items, int TotalCount)> GetCustomerShipmentsAsync(
        Guid customerId, int page, int pageSize,
        string? status = null,
        string? serviceType = null,
        DateTime? dateFrom = null,
        DateTime? dateTo = null);
}
