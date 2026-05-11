using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetCustomerShipments;

public class GetCustomerShipmentsQueryHandler : IRequestHandler<GetCustomerShipmentsQuery, PagedResponse<CustomerShipmentDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IShipmentServiceClient _shipmentServiceClient;

    public GetCustomerShipmentsQueryHandler(
        ICustomerRepository customerRepository,
        IShipmentServiceClient shipmentServiceClient)
    {
        _customerRepository = customerRepository;
        _shipmentServiceClient = shipmentServiceClient;
    }

    public async Task<PagedResponse<CustomerShipmentDto>> Handle(GetCustomerShipmentsQuery request, CancellationToken cancellationToken)
    {
        // If caller is a Customer role, restrict to their own customerId
        var targetCustomerId = request.CallerCustomerId.HasValue
            ? request.CallerCustomerId.Value
            : request.CustomerId;

        var customer = await _customerRepository.GetByIdAsync(targetCustomerId)
            ?? throw new NotFoundException("Customer", targetCustomerId);

        var (items, totalCount) = await _shipmentServiceClient.GetCustomerShipmentsAsync(
            targetCustomerId, request.Page, request.PageSize,
            request.Status, request.ServiceType, request.DateFrom, request.DateTo);

        return PagedResponse<CustomerShipmentDto>.Ok(items, request.Page, request.PageSize, totalCount);
    }
}
