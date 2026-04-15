using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Entities;
using CMS.CustomerService.Domain.Enums;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CMS.CustomerService.Application.Commands.RegisterCustomer;

public class RegisterCustomerCommandHandler : IRequestHandler<RegisterCustomerCommand, ApiResponse<CustomerDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly INotificationServiceClient _notificationClient;
    private readonly IMapper _mapper;
    private readonly ILogger<RegisterCustomerCommandHandler> _logger;

    public RegisterCustomerCommandHandler(
        ICustomerRepository customerRepository,
        INotificationServiceClient notificationClient,
        IMapper mapper,
        ILogger<RegisterCustomerCommandHandler> logger)
    {
        _customerRepository = customerRepository;
        _notificationClient = notificationClient;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<CustomerDto>> Handle(RegisterCustomerCommand command, CancellationToken cancellationToken)
    {
        var exists = await _customerRepository.ExistsAsync(command.Request.Email);
        if (exists)
            throw new ConflictException($"A customer with email '{command.Request.Email}' already exists.");

        if (!Enum.TryParse<CustomerType>(command.Request.Type, ignoreCase: true, out var customerType))
            throw new ValidationException(new[] { $"Invalid customer type: '{command.Request.Type}'. Valid values: {string.Join(", ", Enum.GetNames<CustomerType>())}" });

        var year = DateTime.UtcNow.Year;
        var seq = await _customerRepository.GetNextSequenceAsync(year);
        var customerCode = $"CUST-{year}-{seq:D3}";

        var customer = Customer.Create(
            customerCode,
            command.Request.FullName,
            command.Request.Email,
            command.Request.Phone,
            command.Request.Address,
            command.Request.City,
            command.Request.Country,
            customerType,
            command.Request.CompanyName,
            command.Request.ContactPerson,
            command.Request.State,
            command.Request.ZipCode,
            command.Request.TaxId,
            command.Request.CreditLimit,
            command.Request.PaymentTerms);

        if (command.Request.KycDocuments is not null)
        {
            foreach (var kycDoc in command.Request.KycDocuments)
                customer.AddKycDocument(kycDoc.DocumentType, kycDoc.BlobReference);
        }

        await _customerRepository.AddAsync(customer);

        // Send welcome email (non-blocking)
        try
        {
            await _notificationClient.SendWelcomeEmailAsync(
                customer.Id.ToString(), customer.Email,
                string.IsNullOrWhiteSpace(customer.CompanyName) ? customer.FullName : customer.CompanyName);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to send welcome email to customer {CustomerId}", customer.Id);
        }

        var dto = _mapper.Map<CustomerDto>(customer);
        return ApiResponse<CustomerDto>.Ok(dto, "Customer registered successfully.");
    }
}
