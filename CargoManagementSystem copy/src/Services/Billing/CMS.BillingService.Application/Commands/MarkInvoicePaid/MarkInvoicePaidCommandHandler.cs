using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Domain.Enums;
using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;

namespace CMS.BillingService.Application.Commands.MarkInvoicePaid;

public class MarkInvoicePaidCommandHandler : IRequestHandler<MarkInvoicePaidCommand, ApiResponse<InvoiceDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IMapper _mapper;

    public MarkInvoicePaidCommandHandler(
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IMapper mapper)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _mapper = mapper;
    }

    public async Task<ApiResponse<InvoiceDto>> Handle(MarkInvoicePaidCommand command, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.InvoiceId)
            ?? throw new NotFoundException("Invoice", command.InvoiceId);

        if (invoice.Status == InvoiceStatus.Paid)
            throw new UnprocessableException("Invoice is already marked as Paid.");

        if (invoice.Status == InvoiceStatus.Void)
            throw new UnprocessableException("Cannot mark a voided invoice as Paid.");

        // Create a payment record for the outstanding balance
        var payment = Payment.Create(
            invoice.Id,
            invoice.OutstandingBalance,
            PaymentMethod.BankTransfer,
            command.Request.ReferenceNumber,
            "system",
            command.Request.Notes);

        invoice.MarkPaid(command.Request.ReferenceNumber);

        await _paymentRepository.AddAsync(payment);
        await _invoiceRepository.UpdateAsync(invoice);

        var dto = _mapper.Map<InvoiceDto>(invoice);
        return ApiResponse<InvoiceDto>.Ok(dto, "Invoice marked as paid.");
    }
}
