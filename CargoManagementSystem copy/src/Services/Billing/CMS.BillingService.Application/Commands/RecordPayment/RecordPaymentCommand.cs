using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.RecordPayment;

public class RecordPaymentCommand : IRequest<ApiResponse<PaymentDto>>
{
    public RecordPaymentRequest Request { get; }

    public RecordPaymentCommand(RecordPaymentRequest request)
    {
        Request = request;
    }
}
