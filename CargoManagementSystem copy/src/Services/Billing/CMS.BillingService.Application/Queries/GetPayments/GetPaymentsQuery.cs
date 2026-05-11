using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetPayments;

public record GetPaymentsQuery(
    int Page = 1, int PageSize = 20,
    Guid? InvoiceId = null,
    string? Method = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null) : IRequest<PagedResponse<PaymentDto>>;
