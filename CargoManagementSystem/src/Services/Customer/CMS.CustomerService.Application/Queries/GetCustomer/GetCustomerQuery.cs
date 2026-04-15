using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetCustomer;

public record GetCustomerQuery(Guid CustomerId) : IRequest<ApiResponse<CustomerDto>>;
