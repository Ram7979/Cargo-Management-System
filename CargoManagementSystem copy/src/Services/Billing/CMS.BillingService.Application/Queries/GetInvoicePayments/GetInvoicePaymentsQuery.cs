using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetInvoicePayments;

public record GetInvoicePaymentsQuery(Guid InvoiceId) : IRequest<ApiResponse<IEnumerable<PaymentDto>>>;
