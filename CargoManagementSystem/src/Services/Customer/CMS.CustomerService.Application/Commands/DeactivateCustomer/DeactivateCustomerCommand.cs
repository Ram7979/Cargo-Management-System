using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.DeactivateCustomer;

public record DeactivateCustomerCommand(Guid CustomerId) : IRequest<ApiResponse<bool>>;
