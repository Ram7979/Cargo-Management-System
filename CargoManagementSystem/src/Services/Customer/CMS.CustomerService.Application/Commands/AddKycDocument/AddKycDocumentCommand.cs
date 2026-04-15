using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.AddKycDocument;

public record AddKycDocumentCommand(Guid CustomerId, string DocumentType, string BlobReference) : IRequest<ApiResponse<KycDocumentDto>>;
