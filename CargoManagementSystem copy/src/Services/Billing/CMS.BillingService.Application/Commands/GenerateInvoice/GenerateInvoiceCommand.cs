using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.GenerateInvoice;

public class GenerateInvoiceCommand : IRequest<ApiResponse<InvoiceDto>>
{
    public GenerateInvoiceRequest Request { get; }

    public GenerateInvoiceCommand(GenerateInvoiceRequest request)
    {
        Request = request;
    }
}
