using CMS.CustomerService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Queries.GetKycDocuments;

public record GetKycDocumentsQuery(Guid CustomerId) : IRequest<ApiResponse<IEnumerable<KycDocumentDto>>>;
