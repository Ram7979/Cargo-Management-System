using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.MarkInvoicePaid;

public record MarkInvoicePaidCommand(Guid InvoiceId, MarkPaidRequest Request) : IRequest<ApiResponse<InvoiceDto>>;
