using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.UpdateCustomer;

public record UpdateCustomerCommand(Guid CustomerId, UpdateCustomerRequest Request) : IRequest<ApiResponse<CustomerDto>>;
