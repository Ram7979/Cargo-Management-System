using AutoMapper;
using CMS.CustomerService.Application.DTOs;
using CMS.CustomerService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetKycDocuments;

public class GetKycDocumentsQueryHandler : IRequestHandler<GetKycDocumentsQuery, ApiResponse<IEnumerable<KycDocumentDto>>>
{
    private readonly ICustomerRepository _customerRepository;
    private readonly IKycDocumentRepository _kycDocumentRepository;
    private readonly IMapper _mapper;

    public GetKycDocumentsQueryHandler(
        ICustomerRepository customerRepository,
        IKycDocumentRepository kycDocumentRepository,
        IMapper mapper)
    {
        _customerRepository = customerRepository;
        _kycDocumentRepository = kycDocumentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<IEnumerable<KycDocumentDto>>> Handle(GetKycDocumentsQuery request, CancellationToken cancellationToken)
    {
        var customer = await _customerRepository.GetByIdAsync(request.CustomerId)
            ?? throw new NotFoundException("Customer", request.CustomerId);

        var docs = await _kycDocumentRepository.GetByCustomerIdAsync(request.CustomerId);
        var dtos = _mapper.Map<IEnumerable<KycDocumentDto>>(docs);
        return ApiResponse<IEnumerable<KycDocumentDto>>.Ok(dtos);
    }
}
