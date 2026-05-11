using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Enums;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.UpdateCustomer;

public class UpdateCustomerCommandHandler : IRequestHandler<UpdateCustomerCommand, ApiResponse<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public UpdateCustomerCommandHandler(
        ICustomerRepository customerRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<CustomerDto>> Handle(UpdateCustomerCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId)
            ?? throw new NotFoundException("Customer", command.CustomerId);

        // Update type if provided
        if (!string.IsNullOrWhiteSpace(command.Request.Type))
        {
            if (!Enum.TryParse<CustomerType>(command.Request.Type, ignoreCase: true, out var newType))
                throw new ValidationException(new[] { $"Invalid customer type: '{command.Request.Type}'." });
            customer.UpdateType(newType);
        }

        customer.Update(
            command.Request.FullName,
            command.Request.Phone,
            command.Request.Address,
            command.Request.City,
            command.Request.Country,
            command.Request.CompanyName,
            command.Request.ContactPerson,
            command.Request.State,
            command.Request.ZipCode,
            command.Request.CreditLimit,
            command.Request.PaymentTerms);

        await _customerRepository.UpdateAsync(customer);
        await _cacheService.RemoveAsync($"customer:{command.CustomerId}");

        var dto = _mapper.Map<CustomerDto>(customer);
        return ApiResponse<CustomerDto>.Ok(dto, "Customer updated successfully.");
    }
}
