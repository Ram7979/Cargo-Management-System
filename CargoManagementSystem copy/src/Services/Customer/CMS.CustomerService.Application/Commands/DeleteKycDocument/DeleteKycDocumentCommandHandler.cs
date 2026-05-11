using CMS.CustomerService.Application.Interfaces;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.DeleteKycDocument;

public class DeleteKycDocumentCommandHandler : IRequestHandler<DeleteKycDocumentCommand, ApiResponse<bool>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IKycDocumentRepository _kycDocumentRepository;
    private readonly ICacheService _cacheService;

    public DeleteKycDocumentCommandHandler(
        ICustomerRepository customerRepository,
        IKycDocumentRepository kycDocumentRepository,
        ICacheService cacheService)
    {
        _customerRepository = customerRepository;
        _kycDocumentRepository = kycDocumentRepository;
        _cacheService = cacheService;
    }

    public async Task<ApiResponse<bool>> Handle(DeleteKycDocumentCommand command, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(command.CustomerId)
            ?? throw new NotFoundException("Customer", command.CustomerId);

        var doc = await _kycDocumentRepository.GetByIdAsync(command.DocumentId)
            ?? throw new NotFoundException("KYC Document", command.DocumentId);

        if (doc.CustomerId != command.CustomerId)
            throw new ForbiddenException("Document does not belong to this customer.");

        customer.RemoveKycDocument(command.DocumentId);
        await _customerRepository.UpdateAsync(customer);
        await _kycDocumentRepository.DeleteAsync(command.DocumentId);
        await _cacheService.RemoveAsync($"customer:{command.CustomerId}");

        return ApiResponse<bool>.Ok(true, "KYC document removed successfully.");
    }
}
