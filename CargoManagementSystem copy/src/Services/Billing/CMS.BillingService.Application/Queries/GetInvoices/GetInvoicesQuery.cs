using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetInvoices;

public record GetInvoicesQuery(
    int Page = 1,
    int PageSize = 20,
    string? CustomerId = null,
    string? Status = null,
    DateTime? DateFrom = null,
    DateTime? DateTo = null,
    Guid? ShipmentId = null) : IRequest<PagedResponse<InvoiceDto>>;
