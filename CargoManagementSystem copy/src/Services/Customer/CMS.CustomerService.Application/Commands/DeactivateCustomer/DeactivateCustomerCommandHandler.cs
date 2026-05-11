using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.DeactivateCustomer;

public class DeactivateCustomerCommandHandler : IRequestHandler<DeactivateCustomerCommand, ApiResponse<bool>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;

    public DeactivateCustomerCommandHandler(ICustomerRepository customerRepository, ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<bool>> Handle(DeactivateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId)
            ?? throw new NotFoundException("Customer", command.CustomerId);

        customer.Deactivate();
        await _customerRepository.UpdateAsync(customer);
        await _cacheService.RemoveAsync($"customer:{command.CustomerId}");

        return ApiResponse<bool>.Ok(true, "Customer deactivated successfully.");
    }
}
