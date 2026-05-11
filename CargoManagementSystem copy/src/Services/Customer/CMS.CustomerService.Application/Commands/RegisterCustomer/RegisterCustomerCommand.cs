using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.RegisterCustomer;

public record RegisterCustomerCommand(RegisterCustomerRequest Request) : IRequest<ApiResponse<CustomerDto>>;
