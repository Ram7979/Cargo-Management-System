using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.RefundPayment;

public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IPaymentRepository _paymentRepository;
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IMapper _mapper;

    public RefundPaymentCommandHandler(
        IPaymentRepository paymentRepository,
        IInvoiceRepository invoiceRepository,
        IMapper mapper)
    {
        _paymentRepository = paymentRepository;
        _invoiceRepository = invoiceRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(RefundPaymentCommand command, CancellationToken cancellationToken)
    {
        var originalPayment = await _paymentRepository.GetByIdAsync(command.Request.PaymentId)
            ?? throw new NotFoundException("Payment", command.Request.PaymentId);

        if (originalPayment.IsRefund)
            throw new UnprocessableException("Cannot refund a refund payment.");

        if (command.Request.Amount > Math.Abs(originalPayment.Amount))
            throw new UnprocessableException($"Refund amount cannot exceed original payment amount ({originalPayment.Amount:C}).");

        var invoice = await _invoiceRepository.GetByIdAsync(originalPayment.InvoiceId)
            ?? throw new NotFoundException("Invoice", originalPayment.InvoiceId);

        var refund = Payment.CreateRefund(
            originalPayment.InvoiceId,
            command.Request.Amount,
            originalPayment.Method,
            $"REFUND-{originalPayment.ReferenceNumber}",
            command.Request.Reason);

        // Reverse the payment on the invoice
        invoice.ApplyPayment(-command.Request.Amount); // negative = increase outstanding balance

        await _paymentRepository.AddAsync(refund);
        await _invoiceRepository.UpdateAsync(invoice);

        var dto = _mapper.Map<PaymentDto>(refund);
        return ApiResponse<PaymentDto>.Ok(dto, "Refund processed successfully.");
    }
}
