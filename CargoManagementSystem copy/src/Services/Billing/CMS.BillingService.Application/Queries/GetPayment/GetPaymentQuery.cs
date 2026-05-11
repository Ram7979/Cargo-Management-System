using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Queries.GetPayment;

public class GetPaymentQuery : IRequest<ApiResponse<PaymentDto>>
{
    public Guid PaymentId { get; }

    public GetPaymentQuery(Guid paymentId)
    {
        PaymentId = paymentId;
    }
}
