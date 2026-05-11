using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetOverdueInvoices;

public record GetOverdueInvoicesQuery : IRequest<ApiResponse<IEnumerable<InvoiceDto>>>;
