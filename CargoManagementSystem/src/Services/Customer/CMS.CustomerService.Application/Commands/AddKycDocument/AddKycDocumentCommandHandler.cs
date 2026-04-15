using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.AddKycDocument;

public class AddKycDocumentCommandHandler : IRequestHandler<AddKycDocumentCommand, ApiResponse<KycDocumentDto>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly ICacheService _cacheService;
    private readonly IMapper _mapper;

    public AddKycDocumentCommandHandler(
        ICustomerRepository customerRepository,
        ICacheService cacheService,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _cacheService = cacheService;
        _mapper = mapper;
    }

    public async Task<ApiResponse<KycDocumentDto>> Handle(AddKycDocumentCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId)
            ?? throw new NotFoundException("Customer", command.CustomerId);

        customer.AddKycDocument(command.DocumentType, command.BlobReference);
        await _customerRepository.UpdateAsync(customer);
        await _cacheService.RemoveAsync($"customer:{command.CustomerId}");

        var doc = customer.KycDocuments.Last();
        var dto = _mapper.Map<KycDocumentDto>(doc);
        return ApiResponse<KycDocumentDto>.Ok(dto, "KYC document added successfully.");
    }
}
