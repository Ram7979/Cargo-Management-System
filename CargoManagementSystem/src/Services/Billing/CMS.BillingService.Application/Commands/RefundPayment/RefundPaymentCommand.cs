using CMS.BillingService.Application.DTOs;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.RefundPayment;

public record RefundPaymentCommand(RefundPaymentRequest Request) : IRequest<ApiResponse<PaymentDto>>;
