using System.Security.Claims;
using AutoMapper;
using CMS.BillingService.Application.DTOs;
using CMS.BillingService.Application.Interfaces;
using CMS.BillingService.Domain.Entities;
using CMS.BillingService.Domain.Enums;
using CMS.BillingService.Domain.Interfaces;
using CMS.Shared.Exceptions;
using CMS.Shared.Responses;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace CMS.BillingService.Application.Commands.RecordPayment;

public class RecordPaymentCommandHandler : IRequestHandler<RecordPaymentCommand, ApiResponse<PaymentDto>>
{
    private readonly IInvoiceRepository _invoiceRepository;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IBlobService _blobService;
    private readonly INotificationServiceClient _notificationClient;
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly IMapper _mapper;
    private readonly ILogger<RecordPaymentCommandHandler> _logger;

    public RecordPaymentCommandHandler(
        IInvoiceRepository invoiceRepository,
        IPaymentRepository paymentRepository,
        IBlobService blobService,
        INotificationServiceClient notificationClient,
        IHttpContextAccessor httpContextAccessor,
        IMapper mapper,
        ILogger<RecordPaymentCommandHandler> logger)
    {
        _invoiceRepository = invoiceRepository;
        _paymentRepository = paymentRepository;
        _blobService = blobService;
        _notificationClient = notificationClient;
        _httpContextAccessor = httpContextAccessor;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<ApiResponse<PaymentDto>> Handle(RecordPaymentCommand command, CancellationToken cancellationToken)
    {
        var invoice = await _invoiceRepository.GetByIdAsync(command.Request.InvoiceId)
            ?? throw new NotFoundException("Invoice", command.Request.InvoiceId);

        if (!invoice.CanAcceptPayment())
            throw new UnprocessableException($"Invoice in status '{invoice.Status}' cannot accept payments.");

        if (!Enum.TryParse<PaymentMethod>(command.Request.Method, ignoreCase: true, out var paymentMethod))
            throw new ValidationException(new[] { $"Invalid payment method: '{command.Request.Method}'. Valid: {string.Join(", ", Enum.GetNames<PaymentMethod>())}" });

        // Overpayment check — allow with forceRecord flag
        if (command.Request.Amount > invoice.OutstandingBalance && !command.Request.ForceRecord)
            throw new UnprocessableException(
                $"Payment amount ({command.Request.Amount:C}) exceeds outstanding balance ({invoice.OutstandingBalance:C}). Set forceRecord=true to confirm overpayment.");

        var actorId = _httpContextAccessor.HttpContext?.User?.FindFirstValue(ClaimTypes.NameIdentifier) ?? "system";

        var payment = Payment.Create(
            invoice.Id,
            command.Request.Amount,
            paymentMethod,
            command.Request.ReferenceNumber,
            actorId,
            command.Request.Notes,
            command.Request.PaymentDate);

        invoice.ApplyPayment(command.Request.Amount);

        // Generate receipt for every payment (partial and full)
        var receiptContent = System.Text.Encoding.UTF8.GetBytes(
            $"PAYMENT RECEIPT\n" +
            $"Invoice: {invoice.InvoiceNumber}\n" +
            $"Amount Paid: {command.Request.Amount:C}\n" +
            $"Method: {paymentMethod}\n" +
            $"Reference: {command.Request.ReferenceNumber}\n" +
            $"Date: {payment.PaidAt:yyyy-MM-dd HH:mm}\n" +
            $"Outstanding Balance: {invoice.OutstandingBalance:C}");

        var receiptUrl = await _blobService.UploadAsync("receipts", $"receipt-{payment.Id}.pdf", receiptContent);
        payment.SetReceiptUrl(receiptUrl);

        await _paymentRepository.AddAsync(payment);
        await _invoiceRepository.UpdateAsync(invoice);

        // Email receipt (non-blocking)
        try
        {
            await _notificationClient.SendReceiptEmailAsync(
                invoice.CustomerId.ToString(), "customer@example.com",
                invoice.InvoiceNumber, receiptUrl);
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to email receipt for invoice {InvoiceNumber}", invoice.InvoiceNumber);
        }

        var dto = _mapper.Map<PaymentDto>(payment);
        return ApiResponse<PaymentDto>.Ok(dto, "Payment recorded successfully.");
    }
}
