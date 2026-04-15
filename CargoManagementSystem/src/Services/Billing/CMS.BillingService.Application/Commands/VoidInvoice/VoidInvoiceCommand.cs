using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.VoidInvoice;

public record VoidInvoiceCommand(Guid InvoiceId, VoidInvoiceRequest Request) : IRequest<ApiResponse<InvoiceDto>>;
