using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetInvoice;

public class GetInvoiceQuery : IRequest<ApiResponse<InvoiceDto>>
{
    public Guid InvoiceId { get; }

    public GetInvoiceQuery(Guid invoiceId)
    {
        InvoiceId = invoiceId;
    }
}
