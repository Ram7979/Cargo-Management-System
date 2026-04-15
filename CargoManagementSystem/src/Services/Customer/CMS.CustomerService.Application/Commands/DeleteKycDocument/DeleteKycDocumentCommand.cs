using CMS.Shared.Responses;
using MediatR;

namespace CMS.CustomerService.Application.Commands.DeleteKycDocument;

public record DeleteKycDocumentCommand(Guid CustomerId, Guid DocumentId) : IRequest<ApiResponse<bool>>;
