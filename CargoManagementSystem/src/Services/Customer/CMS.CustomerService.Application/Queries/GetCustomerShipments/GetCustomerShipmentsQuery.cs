using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetCustomerShipments;

public record GetCustomerShipmentsQuery(
    Guid CustomerId,
    int Page,
    int PageSize,
    Guid? CallerCustomerId = null,
    string? Status = null,
    string? ServiceType = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    string? Format = null) : IRequest<PagedResponse<CustomerShipmentDto>>;
